using System.Net;
using System.Net.Sockets;
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

        // DB 조회 예제: 참조된 MSD.Crux.Core.IUserRepo 구현코드를 사용해서 DB 조회
        try
        {
            int userId = 1;
            var user = await _userRepo.GetByIdAsync(userId); // ID가 1인 유저를 가져옴
            if (user != null)
            {
                _logger.LogInformation($"테스트 유저: ID={user.Id}, Name={user.Name}");
            }
            else
            {
                _logger.LogInformation($"유저 ID= {userId} 없음");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "에러남");
        }


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
    /// 클라이언트로부터 데이터를 읽고, 레포지토리에서 유저 정보를 조회한 후 응답을 클라이언트로 전송한다.
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

            while (!stoppingToken.IsCancellationRequested)
            {
                // 클라이언트로부터 데이터 수신 (연결 상태 확인)
                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);

                if (bytesRead == 0)
                {
                    // 클라이언트 연결 종료
                    _logger.LogInformation("Client disconnected.");
                    break;
                }

                // 이곳에서 DB에서 데이터 조회 (예제는 랜덤 데이터로 대체)
                string? lineId = "vi01";
                var time = DateTime.UtcNow;
                int total = new Random().Next(0, 100);

                // 커스텀 프로토콜 (CSV 포맷)
                string? response = $"{lineId},{time:O},{total}";
                byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(response);

                // 클라이언트에 데이터 전송
                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length, stoppingToken);

                // 로그 출력
                _logger.LogInformation($"스트리밍 데이터: {response}");

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
}
