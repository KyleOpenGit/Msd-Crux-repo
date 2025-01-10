using System.CommandLine;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace MSD.DevTool;

class Program
{
    private static string? ConnectionString { get; set; }

    static async Task Main(string[] args)
    {
        // CLI 명령어 정의
        var rootCommand = new RootCommand("MSD.DevTool CLI Tool") { new Option<string>("--connection-string", "Database connection string") };

        rootCommand.SetHandler((string connectionString) =>
                               {
                                   if (string.IsNullOrWhiteSpace(connectionString))
                                   {
                                       Console.WriteLine("[ERROR] Connection string is required. Use --connection-string to provide it.");
                                       Environment.Exit(1);
                                   }

                                   ConnectionString = connectionString;
                               },
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
        Console.WriteLine($"[INFO] Service running with DB connection: {_dbConnection.ConnectionString}");
    }
}
