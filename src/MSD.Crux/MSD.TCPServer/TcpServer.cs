using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSD.Crux.Common;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;
using MSD.Crux.Core.Models.TCP;

namespace MSD.TCPServer;

/// <summary>
/// 백그라운드 프로세스로 TCP 소켓서버를 열고 연결을 기다리며 .
/// </summary>
/// <remarks>IHostedService 구현체  BackgroundService(기본제공)를 상속받은 클래스 구현</remarks>
public class TcpServer : BackgroundService
{
    private readonly int _port;
    private readonly ILogger<TcpServer> _logger;
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;
    private readonly IVisionCumRepo _visionCumRepo;
    private readonly IInjectionCumRepo _injectionCumRepo;

    /// <summary>
    /// 생성자. 객체 생성시 DIC 에 등록된 객체들이 매개변수를 통해 주입된다.
    /// </summary>
    /// <param name="port">DI로 주입되는 포트 넘버</param>
    /// <param name="logger">DI로 주입되는 ILogger 구현체 객체</param>
    /// <param name="userRepo">DI로 주입되는 MSD.Crux.Core 레포지토리 인터페이스를 구현한 객체(MSD.Crux.Infra.UserRepoPsqlDb)</param>
    public TcpServer(int port, ILogger<TcpServer> logger, IUserRepo userRepo, IConfiguration configuration, IVisionCumRepo visionCumRepo, IInjectionCumRepo injectionCumRepo)
    {
        _port = port;
        _logger = logger;
        _userRepo = userRepo;
        _configuration = configuration;
        _visionCumRepo = visionCumRepo;
        _injectionCumRepo = injectionCumRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 지정된 포트에서 클라이언트의 연결을 기다린다.
        TcpListener? listener = new(IPAddress.Any, _port);
        listener.Start();
        _logger.LogInformation($"TCP 소켓 서버 실행. 포트넘버: {_port}");

        // 클라이언트 연결.
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //pending 중인 클라이언트의 연결을 수락하고 각 연결마다 TcpClient 객체 생성.
                TcpClient? client = await listener.AcceptTcpClientAsync(stoppingToken);
                _logger.LogInformation("새로운 Client 연결됨");

                // Task를 await 없이 비동기적으로 실행해서 다음 pending 중인 연결을 곧바로 수락할수 있도록 한다.
                _ = ProcessClientStreamAsync(client, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TCP server 에러");
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>
    /// 연결된  클라이언트로부터의 데이터 스트림을 비동기적으로 처리.
    /// </summary>
    /// <param name="client">연결된 클라이언트를 나타내는 <see cref="TcpClient"/> 인스턴스.</param>
    /// <param name="stoppingToken">데이터 스트림 루프를 중지할 때 사용할 <see cref="CancellationToken"/>.</param>
    /// <returns>비동기 작업을 나타내는 <see cref="Task"/>.</returns>
    /// <remarks>
    /// 클라이언트로부터 데이터를 읽고, 레포지토리에 저장한다.
    /// 클라이언트가 연결을 종료하거나 중지 토큰이 호출되면 연결이 종료.
    /// </remarks>
    /// <remarks>
    /// 이 메서드는 다음의 역할을 수행:
    /// - 클라이언트가 보낸 데이터를 읽고 커스텀 TCP 프로토콜을 기반으로 메시지 헤더와 페이로드를 구분하여 파싱.
    /// - 헤더의 FrameType에 따라 JWT 검증 및 Payload Data를 데이터베이스에 insert.
    /// - 필요한 경우 클라이언트에 적절한 응답을 전송
    /// - 클라이언트 연결이 끊기거나 예외가 발생하면 연결 종료.
    ///
    /// 이 메서드는 비동기로 실행되며, 각 클라이언트 연결마다 독립적으로 동작한다.
    /// </remarks>
    /// <exception cref="Exception">통신 중 발생한 예외는 로그로 기록.</exception>
    private async Task ProcessClientStreamAsync(TcpClient client, CancellationToken stoppingToken)
    {
        // 클라이언트 연결에대한 데이터 송수신 스트림
        await using var networkStream = client.GetStream();
        _logger.LogInformation("-Get Stream!: ProcessClientStreamAsync");


        //연결되어있는동안 네트워크 스트림을 읽어서 처리
        try
        {
            while (!stoppingToken.IsCancellationRequested && client.Connected)
            {
                // 헤더 용 버퍼.커스텀 TCP 프로토콜 frame의 헤더만을을 위한 길이 공간 버퍼.
                // 스트림 데이터를 6바이트만큼만 읽어서 headerBuffer에 넣고, 읽은 데이터 길이가 0이면 연결을 종료한다.
                var (hasDataInHeader, headerBuffer) = await TryReadFromStreamAsync(networkStream, 6, stoppingToken);
                if (!hasDataInHeader)
                {
                    break; // 클라이언트 연결 종료
                }


                // 헤더 파싱
                VpbusFrameHeader frameHeader = new();

                if (Enum.IsDefined(typeof(FrameType), (int)headerBuffer[0]))
                {
                    frameHeader.FrameType = (FrameType)headerBuffer[0];
                }
                else
                {
                    _logger.LogError($"Invalid FrameType: {headerBuffer[0]}");
                    throw new InvalidOperationException($"Invalid FrameType: {headerBuffer[0]}");
                }

                frameHeader.PayloadLength = BitConverter.ToUInt16(headerBuffer, 1);
                frameHeader.DataVersion = headerBuffer[3];
                frameHeader.Role = headerBuffer[4];


                // 페이로드 읽기
                //읽은 바이트 수가 0이면 연결끊김(종료)
                var (hasDataInPayload, payloadBuffer) = await TryReadFromStreamAsync(networkStream, frameHeader.PayloadLength, stoppingToken);
                if (!hasDataInPayload)
                {
                    break; // 클라이언트 연결 종료
                }


                switch (frameHeader.FrameType)
                {
                    case FrameType.Jwt:
                        await ParseJwtTypePayloadAsync(payloadBuffer);
                        break;
                    case FrameType.Injection:
                        await ParseCumTypePayloadAsync(FrameType.Injection, payloadBuffer);
                        break;
                    case FrameType.Vision:
                        await ParseCumTypePayloadAsync(FrameType.Vision, payloadBuffer);
                        break;
                    default:
                        break;
                }

                // 응답 전송
                string responseMessage = "Succeeded";
                byte[] responseBytes = Encoding.ASCII.GetBytes(responseMessage);
                //TODO: await 필요성 재고
                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "클라이언트 데이터 스트림 처리 중 에러 발생");
        }
        finally
        {
            client.Close();
            _logger.LogInformation("클라이언트 종료됨");
        }
    }

    /// <summary>
    /// 지정된 길이만큼 NetworkStream에서 데이터를 비동기적으로 읽고, 성공 여부와 데이터가 담긴 바이트배열 버퍼를 반환
    /// </summary>
    /// <param name="networkStream">데이터를 읽을 대상 <see cref="NetworkStream"/>.</param>
    /// <param name="lengthToRead">읽어야 할 데이터의 바이트 길이.</param>
    /// <param name="stoppingToken">작업 취소를 위한 <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// 읽기 성공 여부와 데이터를 포함하는 튜플.
    /// - <c>success</c>: 데이터를 성공적으로 읽었는지 여부.
    /// - <c>buffer</c>: 읽어온 데이터를 담고 있는 바이트 배열. 실패 시 빈 배열 반환.
    /// </returns>
    /// <remarks>
    /// - 데이터를 지정된 길이만큼 읽고, 실패 시 빈 배열과 false를 반환.
    /// - 성공적으로 데이터를 읽은 경우 true와 데이터를 반환합.
    /// - 클라이언트 연결이 종료되었거나 읽을 데이터가 없으면 success가 false.
    /// </remarks>
    private async Task<(bool success, byte[] buffer)> TryReadFromStreamAsync(NetworkStream networkStream, int lengthToRead, CancellationToken stoppingToken)
    {
        byte[] buffer = new byte[lengthToRead];
        // networkStream에서 마지막으로 읽은 그 다음의 데이터(스트림의 현재 위치)부터 버퍼 길이 만큼 읽어 버퍼의 0번 인덱스부터 채운다.
        // networkStream 은 한번 읽은 부분은 다시 못 읽는다.
        int bytesRead = await networkStream.ReadAsync(buffer, 0, lengthToRead, stoppingToken);

        // 읽은 데이터 길이가 0이면 연결을 종료할 수 있게 success를 false.
        if (bytesRead == 0)
        {
            _logger.LogInformation("Client disconnected by reading zero data from stream.");
            return (false, Array.Empty<byte>()); // (데이터 없음, 빈배열) 반환
        }

        return (true, buffer); // 읽기 성공. (데이터 있음, 데이터가 담긴 버퍼)
    }

    /// <summary>
    /// 페이로드에 담긴 누적 생산량 또는 검사수량 데이터를 데이터베이스에 넣는다.
    /// </summary>
    /// <param name="frameType">커스텀 프로토콜 바이너리구조 frame의 타입</param>
    /// <param name="payloadBuffer">페이로드 바이트배열 버퍼</param>
    private async Task ParseCumTypePayloadAsync(FrameType frameType, byte[] payloadBuffer)
    {
        VpbusFramePayload payload = new()
        {
            LineId = Encoding.ASCII.GetString(payloadBuffer, 0, 4).TrimEnd('\0'),
            Time = BitConverter.ToInt64(payloadBuffer, 4), // 클라이언트에서 보낸 밀리세컨드 값
            LotId = Encoding.ASCII.GetString(payloadBuffer, 12, 20).TrimEnd('\0'),
            Shift = Encoding.ASCII.GetString(payloadBuffer, 32, 4).TrimEnd('\0', ' '),
            EmployeeNumber = BitConverter.ToInt32(payloadBuffer, 36),
            Total = BitConverter.ToInt32(payloadBuffer, 40) // 수정: Total의 시작 위치를 40으로
        };

        // Time 값을 밀리세컨드로 변환하여 DB에 저장
        DateTime timestamp = AddMilliSecondsToUnixEpoch(payload.Time);

        switch (frameType)
        {
            case FrameType.Injection:
                await _injectionCumRepo.AddInjectionCumAsync(new InjectionCum
                {
                    LineId = payload.LineId,
                    Time = timestamp, // 밀리세컨 정밀도 유지된 타임스탬프
                    LotId = payload.LotId,
                    Shift = payload.Shift,
                    EmployeeNumber = payload.EmployeeNumber,
                    Total = payload.Total
                });
                break;
            case FrameType.Vision:
                await _visionCumRepo.AddVisionCumAsync(new VisionCum
                {
                    LineId = payload.LineId,
                    Time = timestamp, // 밀리세컨 정밀도 유지된 타임스탬프
                    LotId = payload.LotId,
                    Shift = payload.Shift,
                    EmployeeNumber = payload.EmployeeNumber,
                    Total = payload.Total
                });
                break;
        }

        return;
    }

    /// <summary>
    /// JWT 토큰 검증용 Frame Type 스트림데이터의 페이로드 데이터를 파싱한다.
    /// </summary>
    /// <param name="payloadBuffer">페이로드 바이트배열 버퍼</param>
    private async Task ParseJwtTypePayloadAsync(byte[] payloadBuffer)
    {
        string jwtToken = Encoding.ASCII.GetString(payloadBuffer).TrimEnd('\0');

        // JWT 유효성 검증
        ClaimsPrincipal? principal = JwtHelper.ValidateToken(jwtToken, _configuration);
        if (principal is null)
        {
            _logger.LogWarning($"[JWT] 토큰이 유효하지 않습니다. 잘못된 토큰 또는 만료. \n 토큰문자열: {jwtToken}");
            return;
        }

        var cruxClaim = CruxClaim.FromClaims(principal.Claims);
        _logger.LogInformation($"[JWT] 인가됨! LoginId: {cruxClaim.LoginId}, Roles: {cruxClaim.Roles}");
    }

    /// <summary>
    /// 밀리세컨 보장 메소드
    /// </summary>
    /// <param name="milliseconds">밀리세컨. 202501312359000</param>
    /// <returns></returns>
    private DateTime AddMilliSecondsToUnixEpoch(long milliseconds)
    {
        // _logger.LogDebug("Converting milliseconds to DateTime. milliseconds: {Milliseconds}", milliseconds); //1736997639139

        // 유닉스 에포크 (1970년 1월 1일 UTC)
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return epoch.AddMilliseconds(milliseconds); // 밀리세컨드 기준으로 DateTime 변환
    }
}
