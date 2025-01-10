using System.CommandLine;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace MSD.DevTool;

class Program
{
    public static string? ConnectionString { get; private set; }

    static async Task Main(string[] args)
    {
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

                                                                         // DI 컨테이너에 서비스 등록 (예시)
                                                                         services.AddScoped<SomeService>();
                                                                     }).Build();
        // DI 테스트: SomeService 사용
        using IServiceScope? scope = host.Services.CreateScope();
        var someService = scope.ServiceProvider.GetRequiredService<SomeService>();
        someService.Run();

        Console.WriteLine("[INFO] Host execution completed.");
    }
}

public class SomeService
{
    private readonly IDbConnection _dbConnection;

    public SomeService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public void Run()
    {
        Console.WriteLine($"[INFO] Service running with DB connection: {_dbConnection.ConnectionString}");
    }
}
