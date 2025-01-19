using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MSD.Crux.Core.Models;

namespace MSD.TCPClient
{
    public class TcpClientHandler
    {
        private readonly string _serverAddress;
        private readonly int _port;

        public TcpClientHandler(string serverAddress, int port)
        {
            _serverAddress = serverAddress;
            _port = port;
        }

        /// <summary>
        /// 서버와 TCP 연결을 설정하고 메시지를 전송
        /// </summary>
        /// <param name="messageBuilder">메시지를 생성하는 함수</param>
        /// <param name="cancellationToken">취소 토큰</param>
        public async Task StartAsync(Func<string, byte[]> messageBuilder, CancellationToken cancellationToken)
        {
            try
            {
                using TcpClient client = new TcpClient();
                Console.WriteLine($"서버에 연결중 {_serverAddress}:{_port}...");
                await client.ConnectAsync(_serverAddress, _port);
                Console.WriteLine("서버에 연결됨!");

                await using NetworkStream stream = client.GetStream();

                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.Write(" FrameType을 입력하세요: (1 for JWT, 2 for VPBUS, or 'exit' to quit): ");
                    string input = Console.ReadLine();

                    if (input?.ToLower() == "exit")
                    {
                        Console.WriteLine("종료...");
                        break;
                    }

                    byte[] message;
                    try
                    {
                        message = messageBuilder(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"메시지 빌더 에러: {ex.Message}");
                        continue;
                    }

                    // 메시지 전송
                    await stream.WriteAsync(message, 0, message.Length, cancellationToken);
                    Console.WriteLine("서버에 메시지 보냄!");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"소켓 에러: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"에러남: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("서버와 연결이 끊어짐");
            }
        }
    }
}
