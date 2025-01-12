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

namespace MSD.Crux.StandAlone.TCP;

/// <summary>
/// 백그라운드 프로세스로 TCP 소켓서버를 열고 리스닝.
/// </summary>
/// <remarks>IHostedService 구현체  BackgroundService(기본제공)를 상속받은 클래스 저의</remarks>
public class TcpServer : BackgroundService
{
    private readonly int _port;
    private readonly ILogger<TcpServer> _logger;
    private readonly IUserRepo _userRepo;
    private readonly IConfiguration _configuration;
    private readonly IVisionCumRepo _visionCumRepo;

    /// <summary>
    /// 생성자. 객체 생성시 DIC 에 등록된 객체들이 매개변수를 통해 주입된다.
    /// </summary>
    /// <param name="port">DI로 주입되는 포트 넘버</param>
    /// <param name="logger">DI로 주입되는 ILogger 구현체 객체</param>
    /// <param name="userRepo">DI로 주입되는 MSD.Crux.Core 레포지토리 인터페이스를 구현한 객체(MSD.Crux.Infra.UserRepoPsqlDb)</param>
    public TcpServer(int port, ILogger<TcpServer> logger, IUserRepo userRepo, IConfiguration configuration, IVisionCumRepo visionCumRepo)
    {
        _port = port;
        _logger = logger;
        _userRepo = userRepo;
        _configuration = configuration;
        _visionCumRepo = visionCumRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TcpListener? listener = new(IPAddress.Any, _port);
        listener.Start();
        _logger.LogInformation($"TCP 소켓 서버 실행. 포트넘버: {_port}");

        // TCP 소켓통신 예시
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                TcpClient? client = await listener.AcceptTcpClientAsync(stoppingToken);
                _logger.LogInformation("Client connected");

                // 별도의 태스크에서 소켓 클라이언트 핸들링
                _ = HandleClientAsync(client, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred in the TCP server.");
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>
    /// 연결된 TCP 클라이언트와의 통신을 처리
    /// </summary>
    /// <param name="client">연결된 클라이언트를 나타내는 <see cref="TcpClient"/> 인스턴스.</param>
    /// <param name="stoppingToken">통신 루프를 중지할 때 사용되는 <see cref="CancellationToken"/>.</param>
    /// <returns>비동기 작업을 나타내는 <see cref="Task"/>.</returns>
    /// <remarks>
    /// 클라이언트로부터 데이터를 읽고, 레포지토리에 저장한다.
    /// 클라이언트가 연결을 종료하거나 중지 토큰이 호출되면 연결이 종료.
    /// </remarks>
    /// <exception cref="Exception">통신 중 발생한 예외는 로그로 기록.</exception>
    private async Task HandleClientAsync(TcpClient client, CancellationToken stoppingToken)
    {
        await using var networkStream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            _logger.LogInformation("Client connection initiated for streaming.");

            while (!stoppingToken.IsCancellationRequested && client.Connected)
            {
                // 헤더 읽기 (6바이트)
                byte[] headerBuffer = new byte[6];
                // 클라이언트로부터 데이터 수신 (연결 상태 확인)
                int headerBytesRead = await networkStream.ReadAsync(headerBuffer, 0, headerBuffer.Length, stoppingToken);


                if (headerBytesRead == 0)
                {
                    // 클라이언트 연결 종료
                    _logger.LogInformation("Client disconnected.");
                    break;
                }

                // 헤더 파싱
                byte frameType = headerBuffer[0];
                ushort messageLength = BitConverter.ToUInt16(headerBuffer, 1); // 2바이트 읽기
                byte messageVersion = headerBuffer[3];
                byte role = headerBuffer[4];
                byte reserved = headerBuffer[5];

                // 로그 출력
                _logger.LogInformation($"[HEADER] FrameType: {frameType}, MessageLength: {messageLength}, MessageVersion: {messageVersion}, Role: {role}");
                // 페이로드 읽기
                byte[] payloadBuffer = new byte[messageLength];
                int payloadBytesRead = await networkStream.ReadAsync(payloadBuffer, 0, payloadBuffer.Length, stoppingToken);

                if (payloadBytesRead == 0)
                {
                    _logger.LogInformation("Client disconnected during payload read.");
                    break;
                }

                if (frameType == 1)
                {
                    HandleJWT(payloadBuffer);
                }
                else if (frameType == 2)
                {
                    VisionCum visinoCum = HandlePayload(payloadBuffer);
                    await _visionCumRepo.AddVisionCumAsync(visinoCum);
                }

                // 응답 전송
                string responseMessage = "Data received successfully";
                byte[] responseBytes = Encoding.ASCII.GetBytes(responseMessage);
                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length, stoppingToken);

                // 1초 대기
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "스트리밍 중 에러 발생");
        }
        finally
        {
            client.Close();
            _logger.LogInformation("Client fully disconnected.");
        }
    }

    /// <summary>
    /// FrameType 값이 2일 때 payload를 파싱
    /// </summary>
    private VisionCum HandlePayload(byte[] payload)
    {
        string lineId = Encoding.ASCII.GetString(payload, 0, 4).TrimEnd('\0');
        long time = BitConverter.ToInt64(payload, 4);
        string lotId = Encoding.ASCII.GetString(payload, 12, 20).TrimEnd('\0');
        string shift = Encoding.ASCII.GetString(payload, 32, 4).TrimEnd('\0');
        long employeeNumber = BitConverter.ToInt64(payload, 36);
        int total = BitConverter.ToInt32(payload, 46);

        _logger.LogInformation($"[PAYLOAD] LineId: {lineId}, Time: {time}, LotId: {lotId}, Shift: {shift}, EmployeeNumber: {employeeNumber}, Total: {total}");
        return new VisionCum { LineId = lineId, Time = ConvertUnixTimeToDateTime(time), LotId = lotId, Shift = shift, EmployeeNumber = (int)employeeNumber, Total = total };
    }

    /// <summary>
    /// FrameType 값이 1일 때 payload를 파싱
    /// </summary>
    private void HandleJWT(byte[] payload)
    {
        string jwtToken = Encoding.ASCII.GetString(payload).TrimEnd('\0');

        // JWT 유효성 검증
        ClaimsPrincipal? principal = JwtHelper.ValidateToken(jwtToken, _configuration);
        if (principal is null)
        {
            _logger.LogWarning($"[JWT] 토큰이 유효하지 않습니다. 잘못된 토큰 또는 만료. \n 토큰문자열: {jwtToken}");
            return;
        }

        var cruxClaim = CruxClaim.FromClaims(principal.Claims);
        _logger.LogInformation($"[JWT] Authentication successful. LoginId: {cruxClaim.LoginId}, Roles: {cruxClaim.Roles}");
    }

    /// <summary>
    /// 유닉스 타임스탬프를 DateTime형식으로 변경
    /// </summary>
    public static DateTime ConvertUnixTimeToDateTime(long unixTime)
    {
        // 유닉스 에포크 (1970년 1월 1일 UTC)
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // 초 단위 타임스탬프를 에포크에 더하여 DateTime 반환
        return epoch.AddSeconds(unixTime);
    }
}
