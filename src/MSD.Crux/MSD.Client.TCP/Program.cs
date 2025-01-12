using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using MSD.Crux.Common;
using MSD.Crux.Core.Models;

namespace MSD.Client.TCP
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Socket Client Test Application");
            // 서버 정보
            string serverAddress = "127.0.0.1"; // 로컬 서버 주소
            int port = 51900; // 서버와 동일한 포트 번호 사용

            // 클라이언트 소켓 생성
            using TcpClient client = new TcpClient();

            VisionCum visionCum = new VisionCum { LineId = "vp1", Time = DateTime.UtcNow, LotId = "AAAA120250107-1", Shift = "A", EmployeeNumber = 202340014, Total = 1000 };

            User user = new User()
            {
                Id = 30,
                EmployeeNumber = 202340014,
                LoginId = "nati",
                LoginPw = "ix83VPAon+7AjG0JPkavXDaHtyYCeObFml4iI6WQK2w=",
                Salt = "eY4uyzWqHi8BeVyl6BbV2w",
                Name = "나띠 욘따라락",
                Roles = "vision"
            };

            try
            {
                // 서버에 연결
                Console.WriteLine($"Connecting to server {serverAddress}:{port}...");
                await client.ConnectAsync(serverAddress, port);
                Console.WriteLine("Connected to server!");

                using NetworkStream stream = client.GetStream();

                while (true)
                {
                    Console.Write("Enter FrameType (1 for JWT, 2 for VPBUS, or 'exit' to quit): ");
                    string input = Console.ReadLine();

                    if (input?.ToLower() == "exit")
                    {
                        Console.WriteLine("Exiting...");
                        break;
                    }

                    if (!byte.TryParse(input, out byte frameType) || (frameType != 1 && frameType != 2))
                    {
                        Console.WriteLine("Invalid FrameType. Please enter 1 or 2.");
                        continue;
                    }

                    byte[] message;

                    if (frameType == 2)
                    {
                        // FrameType 1: 하드코딩된 페이로드 생성
                        message = CreatePayloadMessage(frameType, visionCum);
                    }
                    else if (frameType == 1)
                    {
                        // FrameType 2: JWT 생성 (생성 로직은 비워 둠)
                        Console.WriteLine("FrameType 2 selected. Generating JWT...");
                        message = CreateJWTMessage(frameType, user); // JWT 생성 로직 추가 필요
                    }
                    else
                    {
                        Console.WriteLine("Unsupported FrameType.");
                        continue;
                    }

                    // 헤더 출력
                    Console.WriteLine("\nHeader:");
                    Console.WriteLine($"FrameType: {message[0]}");
                    Console.WriteLine($"MessageLength: {message[1]}");
                    Console.WriteLine($"MessageVersion: {message[2]}");
                    Console.WriteLine($"Role: {message[3]}");
                    Console.WriteLine($"Reserved: {message[4]}");

                    // 페이로드 출력
                    Console.WriteLine("\nPayload:");
                    string lineId = Encoding.ASCII.GetString(message, 5, 4).TrimEnd('\0');
                    Console.WriteLine($"LineId: {lineId}");

                    long time = BitConverter.ToInt64(message, 9);
                    Console.WriteLine($"Time: {time}");

                    string lotId = Encoding.ASCII.GetString(message, 17, 20).TrimEnd('\0');
                    Console.WriteLine($"LotId: {lotId}");

                    string shift = Encoding.ASCII.GetString(message, 37, 4).TrimEnd();
                    Console.WriteLine($"Shift: {shift}");

                    long employeeNumber = BitConverter.ToInt64(message, 41);
                    Console.WriteLine($"EmployeeNumber: {employeeNumber}");

                    int total = BitConverter.ToInt32(message, 51);
                    Console.WriteLine($"Total: {total}");

                    // 메시지 전송
                    await stream.WriteAsync(message, 0, message.Length);
                    Console.WriteLine("Message sent to server.");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Disconnected from server.");
            }
        }

        // FrameType 1: 하드코딩된 페이로드 생성
        private static byte[] CreatePayloadMessage(byte frameType, VisionCum visionCum)
        {
            int payloadSize = 50;
            // 전체 메시지 크기: 헤더(6바이트) + 페이로드(50바이트)
            int totalSize = 6 + payloadSize; // 56 바이트
            byte[] message = new byte[totalSize];

            // Header (6 bytes)
            message[0] = frameType; // FrameType
            //1,2 는 MessageLength (페이로드 크기. 2byte 필요)
            BitConverter.GetBytes((ushort)payloadSize).CopyTo(message, 1);
            message[3] = 1; // MessageVersion
            message[4] = 1; // Role (0=생산)
            message[5] = 0; // Reserved

            // Payload
            Encoding.ASCII.GetBytes(visionCum.LineId.PadRight(4, '\0')).CopyTo(message, 6); // LineId (4 bytes)
            BitConverter.GetBytes(new DateTimeOffset(visionCum.Time).ToUnixTimeSeconds()).CopyTo(message, 10); // Time (8 bytes)
            Encoding.ASCII.GetBytes(visionCum.LotId?.PadRight(20, '\0') ?? new string('\0', 20)).CopyTo(message, 18); // LotId (20 bytes)
            Encoding.ASCII.GetBytes(visionCum.Shift?.PadRight(4, ' ') ?? "    ").CopyTo(message, 38); // Shift (4 bytes)
            BitConverter.GetBytes((visionCum.EmployeeNumber ?? 0)).CopyTo(message, 42); // EmployeeNumber (10 bytes)
            BitConverter.GetBytes(visionCum.Total).CopyTo(message, 50); // Total (4 bytes)

            return message;
        }

        // FrameType 2: JWT 메시지 생성 (JWT 생성 로직은 비워둠)
        private static byte[] CreateJWTMessage(byte frameType, User user)
        {
            string token = "yJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6InN0cmluZyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJzdHJpbmciLCJFbXBsb3llZU51bWJlciI6IjIwMjQ0MDQ1NiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImluamVjdGlvbiIsImV4cCI6MTczNjc0MTA1OCwiaXNzIjoiTVNEIENydXgiLCJhdWQiOiJNU0QgQ2xpZW50In0.UOOrSamvpDOK7vKmtPxeFUsmvIwJQGek6S5NqAuy_X20-n8AUnV572W82yntrJqg7pxoC4JJAMcqv6C9gM4nnpNZW5JSSL9oHMU2Z38qoSgWQBfC8T7URf9Q5GCIW1ni0GJR7lfpZ2C3GcA7ZDLsVjWIwBEi0w8-TwES2dK9YzHhu2_ve0yjE-AWQ_yl1rbqDExJUIO6Dfryf7zHdBRzryEZcfyJNk0t_zVNEs2PCLJGWX4sL91au-k4H50PLO24Jc_xW1JL_WkRJNW1lntKqcsY-8po2xGpKsHVZQJwN-BQNBblC5dsp-509B5ftNMW1zoxPQhMntM2-ld2ScQtTQ";

            byte[] jwtBytes = Encoding.UTF8.GetBytes(token);

            // JWT 메시지의 길이를 확인 (200~1024 바이트 제한 체크)
            if (jwtBytes.Length < 200 || jwtBytes.Length > 1024)
            {
                throw new ArgumentException($"JWT의 길이는 200~1024 바이트여야 합니다. 현재 길이: {jwtBytes.Length}");
            }

            // 전체 메시지 크기: Header(6 바이트) + Payload(JWT 크기)
            int totalLength = 6 + jwtBytes.Length;
            byte[] message = new byte[totalLength];

            // Header 생성
            message[0] = frameType; // FrameType (0: JWT, 1: 생산CUM, 2: 비전CUM)
            BitConverter.GetBytes((ushort)jwtBytes.Length).CopyTo(message, 1); // 2Byte: 토큰길이는 700글자 이상이고 1 byte가 표현 가능한 길이는 0~255이 한계이므로 2Byte(ushort)로 길이를 표현해야한다.
            message[3] = 1; // MessageVersion
            message[4] = 1; // Role (0: 생산, 1: 품질, 2: SCADA)
            message[5] = 0; // Reserved (스페어)

            // Payload(JWT) 복사
            Array.Copy(jwtBytes, 0, message, 6, jwtBytes.Length);

            return message;
        }
    }
}
