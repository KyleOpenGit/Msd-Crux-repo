using System.CommandLine;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSD.Crux.DevTool.Commands;
using MSD.Crux.DevTool.Services;
using Npgsql;

namespace MSD.Crux.DevTool;

class Program
{
    static async Task Main(string[] args)
    {
        // 루트 명령어 옵션 생성 및 실행
        var appRootCommand = new AppRootCommand();
        RootCommand rootCommand = appRootCommand.CreateRootCommand();
        await rootCommand.InvokeAsync(args);


        if (string.IsNullOrWhiteSpace(appRootCommand.ConnectionString))
        {
            Console.WriteLine("[ERROR] Connection string 이 설정되지 않았습니다.");
            return;
        }

        // 제네릭 호스트 생성 및 구성
        IHost? host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                                                                        {
                                                                            // IConfiguration 주입 가능
                                                                            var configuration = hostContext.Configuration;
                                                                            services.AddSingleton<IConfiguration>(configuration);

                                                                            // Npgsql을 사용한 DbConnection 주입
                                                                            services.AddTransient<IDbConnection>(sp =>
                                                                                                                 {
                                                                                                                     return new NpgsqlConnection(appRootCommand.ConnectionString);
                                                                                                                 });

                                                                            // DI 컨테이너에 서비스 등록
                                                                            services.AddScoped<TestingService>();
                                                                        }).Build();
        // DI 테스트
        using IServiceScope? scope = host.Services.CreateScope();
        var someService = scope.ServiceProvider.GetRequiredService<TestingService>();
        someService.Run();
    }
}
