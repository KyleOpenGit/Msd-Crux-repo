using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Shared;
using MSD.TCPServer;
using Npgsql;

// 환경이 없으면 "Production"환경
string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

// 제네릭 호스트
IHost? host = Host.CreateDefaultBuilder(args)
                  // 호스트에 대한 사용자 설정 부분
                  .ConfigureAppConfiguration((hostContext, config) =>
                                             {
                                                 // appsettings.json 구성 파일 로드
                                                 config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                       .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                                             })
                  // DI Container 설정
                  .ConfigureServices((hostContext, services) =>
                                     {
                                         IConfiguration configuration = hostContext.Configuration;

                                         // 환경 변수에서 포트 번호 읽기, 없으면 기본값 사용
                                         int port = int.TryParse(Environment.GetEnvironmentVariable("PORT"), out int parsedPort) ? parsedPort : 51900;

                                         // 커스텀 TCP 소켓 서버 클래스 등록
                                         services.AddHostedService(sp => new TcpServer(port,
                                                                                       sp.GetRequiredService<ILogger<TcpServer>>(),
                                                                                       sp.GetRequiredService<IUserRepo>(),
                                                                                       configuration: configuration,
                                                                                       sp.GetRequiredService<IVisionCumRepo>(),
                                                                                       sp.GetRequiredService<IInjectionCumRepo>()));

                                         // Npgsql을 이용한 DB 커넥션
                                         services.AddTransient<IDbConnection>(sp =>
                                                                              {
                                                                                  string? connectionString = configuration.GetConnectionString("Postgres");
                                                                                  return new NpgsqlConnection(connectionString);
                                                                              });

                                         // 참조된 Crux 서버앱의 코드 사용. 서비스와 레포지토리 구현체 사용 (Shared 확장 메서드로 서비스와 레포지토리를 등록)
                                         services.AddCruxServicesAll();
                                         services.AddCruxRepositoriesAll();
                                     })
                  // 로깅 설정
                  .ConfigureLogging(logging =>
                                    {
                                        logging.ClearProviders();
                                        logging.AddConsole();
                                    })
                  // 호스트 빌드
                  .Build();

// Run the Host!
await host.RunAsync();
