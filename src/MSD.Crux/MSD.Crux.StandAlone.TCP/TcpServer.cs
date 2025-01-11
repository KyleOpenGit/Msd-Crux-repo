using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSD.Crux.Core.IRepositories;

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

    /// <summary>
    /// 생성자. 객체 생성시 DIC 에 등록된 객체들이 매개변수를 통해 주입된다.
    /// </summary>
    /// <param name="port">DI로 주입되는 포트 넘버</param>
    /// <param name="logger">DI로 주입되는 ILogger 구현체 객체</param>
    /// <param name="userRepo">DI로 주입되는 MSD.Crux.Core 레포지토리 인터페이스를 구현한 객체(MSD.Crux.Infra.UserRepoPsqlDb)</param>
    public TcpServer(int port, ILogger<TcpServer> logger, IUserRepo userRepo, IConfiguration configuration)
    {
        _port = port;
        _logger = logger;
        _userRepo = userRepo;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var listener = new TcpListener(IPAddress.Any, _port);
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
        using var networkStream = client.GetStream();
        byte[] buffer = new byte[1024];

        // 1초마다 DB 조회 후 클라이언트에 스트리밍
        try
        {
            _logger.LogInformation("Client connection initiated for streaming.");

            while (!stoppingToken.IsCancellationRequested && client.Connected)
            {
                // 헤더 읽기 (8바이트)
                byte[] headerBuffer = new byte[5];
                // 클라이언트로부터 데이터 수신 (연결 상태 확인)
                int headerBytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);


                if (headerBytesRead == 0)
                {
                    // 클라이언트 연결 종료
                    _logger.LogInformation("Client disconnected.");
                    break;
                }

                // 헤더 파싱
                byte frameType = headerBuffer[0];
                byte messageLength = headerBuffer[1];
                byte messageVersion = headerBuffer[2];
                byte role = headerBuffer[3];
                byte reserved = headerBuffer[4];

                // 로그 출력
                _logger.LogInformation($"[HEADER] FrameType: {frameType}, MessageLength: {messageLength}, MessageVersion: {messageVersion}, Role: {role}");

                // 페이로드 읽기
                byte[] payloadBuffer = new byte[messageLength - 5];
                int payloadBytesRead = await networkStream.ReadAsync(payloadBuffer, 0, payloadBuffer.Length, stoppingToken);

                if (payloadBytesRead == 0)
                {
                    _logger.LogInformation("Client disconnected during payload read.");
                    break;
                }

                // 페이로드 파싱
                string lineId = Encoding.ASCII.GetString(payloadBuffer, 0, 4).TrimEnd('\0'); // LineId (4바이트)
                byte[] timeBytes = new byte[8];
                Array.Copy(payloadBuffer, 4, timeBytes, 0, 8); // Time (8바이트)
                long time = BitConverter.ToInt64(timeBytes, 0);

                string lotId = Encoding.ASCII.GetString(payloadBuffer, 12, 10).TrimEnd('\0'); // LotId (10바이트)
                string shift = Encoding.ASCII.GetString(payloadBuffer, 22, 4).TrimEnd('\0'); // Shift (4바이트)
                long employeeNumber = BitConverter.ToInt64(payloadBuffer, 26); // EmployeeNumber (8바이트)
                int total = BitConverter.ToInt32(payloadBuffer, 34); // Total (4바이트)

                _logger.LogInformation($"[PAYLOAD] Time: {time}, LotId: {lotId}, Shift: {shift}, EmployeeNumber: {employeeNumber}, Total: {total}");

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
    /// 연결된 TCP 클라이언트에서 JWT를 체크
    /// </summary>
    /// <param name="client">연결된 클라이언트를 나타내는 <see cref="TcpClient"/> 인스턴스.</param>
    /// <param name="stoppingToken">통신 루프를 중지할 때 사용되는 <see cref="CancellationToken"/>.</param>
    /// <remarks>
    /// 클라이언트로부터 데이터를 읽고, JWT의 유효성을 체크한다.
    /// </remarks>
    /// <exception cref="Exception">통신 중 발생한 예외는 로그로 기록.</exception>
    private async Task<bool> AuthenticateClientAsync(TcpClient client, CancellationToken stoppingToken)
    {
        try
        {
            NetworkStream stream = client.GetStream();

            // 1. JWT 헤더 읽기 (5바이트)
            byte[] headerBuffer = new byte[5];
            int headerBytesRead = await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length, stoppingToken);

            if (headerBytesRead == 0)
            {
                _logger.LogInformation("[JWT ERROR] Client disconnected before sending header.");
                return false;
            }

            // 헤더 파싱
            byte frameType = headerBuffer[0];
            byte messageLength = headerBuffer[1];
            byte messageVersion = headerBuffer[2];
            byte role = headerBuffer[3];
            byte reserved = headerBuffer[4];

            _logger.LogInformation($"[JWT HEADER] FrameType: {frameType}, MessageLength: {messageLength}, MessageVersion: {messageVersion}, Role: {role}");

            // 2. JWT 페이로드 읽기
            // 메시지 길이는 헤더를 포함한 전체 길이이므로 헤더 크기(5)를 뺀 값을 구함
            int payloadLength = messageLength - 5;
            if (payloadLength <= 0 || payloadLength > 500) // JWT 프로토콜에서 최대 크기 검증 (200~500 범위)
            {
                _logger.LogInformation("[JWT ERROR] Invalid payload length.");
                return false;
            }

            byte[] jwtBuffer = new byte[payloadLength];
            int jwtBytesRead = await stream.ReadAsync(jwtBuffer, 0, jwtBuffer.Length, stoppingToken);

            if (jwtBytesRead == 0)
            {
                _logger.LogInformation("[JWT ERROR] Client disconnected during payload read.");
                return false;
            }

            // JWT 데이터 추출
            string jwtToken = Encoding.ASCII.GetString(jwtBuffer).TrimEnd('\0');
            _logger.LogInformation($"[JWT PAYLOAD] Token: {jwtToken}");

            /*********************************
             *추후에 실제 로직이 들어가야 함 *
             *********************************/
            if (string.IsNullOrEmpty(jwtToken) || jwtToken.Length < 10)
            {
                Console.WriteLine("[JWT ERROR] Invalid JWT token.");
                return false;
            }

            // 실제 검증 로직 (여기에 JWT 서명 검증 또는 유효성 검사를 추가)
            // 예: JwtHelper.VerifyToken(jwtToken);

            _logger.LogInformation("[JWT] Authentication successful.");
            return true;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "TCP소켓 JWT인증 중 에러발생");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TCP소켓 JWT인증 중 에러발생");
            return false;
        }
    }
}
