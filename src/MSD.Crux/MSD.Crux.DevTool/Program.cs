using System.CommandLine;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace MSD.Crux.DevTool;

class Program
{
    private static string? ConnectionString { get; set; }

    static async Task Main(string[] args)
    {
        // 애플리케이션 버전 정보 가져오기
        Assembly? assembly = Assembly.GetExecutingAssembly();
        string? version = assembly.GetName().Version?.ToString() ?? "Unknown";
        string? buildDate = new FileInfo(assembly.Location).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");

        // --app-version 옵션 정의
        Option<bool>? appVersionOption = new Option<bool>("--app-version", "앱 버전 정보를 출력합니다");

        // 루트 명령어 정의. RootCommand() 생성자를 통해 칠드런 심벌 추가
        RootCommand rootCommand = new RootCommand("MSD.Crux.DevTool CLI Tool")
                                  {
                                      //Option<bool> 타입
                                      appVersionOption, new Option<string>("--connection-string", "Database connection string")
                                  };

        rootCommand.SetHandler((bool appVersion, string connectionString) =>
                               {
                                   if (appVersion)
                                   {
                                       Console.WriteLine($"MSD.Crux.DevTool Version: {version}");
                                       Console.WriteLine($"Build Date: {buildDate}");
                                       return;
                                   }

                                   if (string.IsNullOrWhiteSpace(connectionString))
                                   {
                                       Console.WriteLine("[ERROR] Connection string 은 필수입니다..  --connection-string 옵션을 사용하세요");
                                       return;
                                   }

                                   ConnectionString = connectionString;
                                   Console.WriteLine($"[INFO] Connection string set to: {connectionString}");
                               },
                               rootCommand.Children.OfType<Option<bool>>().First(),
                               rootCommand.Children.OfType<Option<string>>().First());

        // CLI 명령어 실행
        await rootCommand.InvokeAsync(args);

        // 제네릭 호스트 생성 및 구성
        var host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
                                                                     {
                                                                         // IConfiguration 주입 가능
                                                                         var configuration = hostContext.Configuration;
                                                                         services.AddSingleton<IConfiguration>(configuration);

                                                                         // Npgsql을 사용한 DbConnection 주입
                                                                         services.AddTransient<IDbConnection>(sp =>
                                                                                                              {
                                                                                                                  string connectionString = Program.ConnectionString;
                                                                                                                  return new NpgsqlConnection(connectionString);
                                                                                                              });

                                                                         // DI 컨테이너에 서비스 등록
                                                                         services.AddScoped<ExampleService>();
                                                                     }).Build();
        // DI 테스트: ExampleService 사용
        using IServiceScope? scope = host.Services.CreateScope();
        var someService = scope.ServiceProvider.GetRequiredService<ExampleService>();
        someService.Run();
    }
}

public class ExampleService
{
    private readonly IDbConnection _dbConnection;

    public ExampleService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public void Run()
    {
        //DI 주입된 IDbConnection 테스트
        if (string.IsNullOrWhiteSpace(_dbConnection.ConnectionString))
        {
            Console.WriteLine("[ERROR] Connection string is required.");
        }
        else
        {
            Console.WriteLine($"[INFO] Service running with DB connection: {_dbConnection.ConnectionString}");
        }
    }
}
