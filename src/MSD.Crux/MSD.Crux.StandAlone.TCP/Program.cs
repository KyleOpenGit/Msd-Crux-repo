using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Infra.Repositories;
using MSD.Crux.Infra.Services;
using MSD.Crux.StandAlone.TCP;
using Npgsql;

//환경이 없으면 "Production"환경
string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

//제네릭 호스트
IHost? host = Host.CreateDefaultBuilder(args)
                  //호스트에대한 사용자 설정 부분
                  .ConfigureAppConfiguration((hostContext, config) =>
                                             {
                                                 // appsettings.json 구성파일 로드
                                                 config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                                       .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                                             })
                  // DI Container 설정
                  .ConfigureServices((hostContext, services) =>
                                     {
                                         // 커스텀 TCP 소켓 서버 클래스 등록
                                         services.AddHostedService(sp => new TcpServer(51900, sp.GetRequiredService<ILogger<TcpServer>>(), sp.GetRequiredService<IUserRepo>()));
                                         // Npgsql을 이용한 Db 커넥션
                                         services.AddTransient<IDbConnection>(sp =>
                                                                              {
                                                                                  string? connectionString = hostContext.Configuration.GetConnectionString("Postgres");
                                                                                  return new NpgsqlConnection(connectionString);
                                                                              });
                                         // 참조된 Crux 서버앱의 코드 사용. 서비스와 레포지토리 구현체 사용
                                         services.AddTransient<IEmployeeRepo, EmployeeRepoPsqlDb>();
                                         services.AddTransient<IUserRepo, UserRepoPsqlDb>();
                                         services.AddScoped<IEmployeeService, EmployeeService>();
                                         services.AddScoped<IUserService, UserService>();
                                         services.AddScoped<IUserLoginService, UserLoginService>();
                                     })
                  //로깅 설정
                  .ConfigureLogging(logging =>
                                    {
                                        logging.ClearProviders();
                                        logging.AddConsole();
                                    })
                  //호스트 빌드
                  .Build();

// Run the Host!
await host.RunAsync();
