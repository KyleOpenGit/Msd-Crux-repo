using System.CommandLine;
using System.Reflection;

namespace MSD.Crux.DevTool.Commands;

public class AppRootCommand
{
    public string? ConnectionString { get; private set; } // 반환할 ConnectionString

    public RootCommand CreateRootCommand()
    {
        // 애플리케이션 버전 정보 가져오기
        Assembly? assembly = Assembly.GetExecutingAssembly();
        string? version = assembly.GetName().Version?.ToString() ?? "Unknown";
        string? buildDate = new FileInfo(assembly.Location).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");


        RootCommand rootCommand = new RootCommand("MSD.Crux.DevTool CLI Tool")
                                  {
                                      new Option<bool>("--app-version", "앱 버전 정보를 출력합니다"), new Option<string>("--connection-string", "Database connection string")
                                  };

        // 커맨드 옵션별 실행 코드
        rootCommand.SetHandler((bool appVersion, string connectionString) =>
                               {
                                   // "--app-version"
                                   if (appVersion)
                                   {
                                       Console.WriteLine($"MSD.Crux.DevTool Version: {version}");
                                       Console.WriteLine($"Build Date: {buildDate}");
                                       return;
                                   }

                                   // "--connection-string"
                                   if (string.IsNullOrWhiteSpace(connectionString))
                                   {
                                       Console.WriteLine("[ERROR] Connection string 은 필수입니다..  --connection-string 옵션을 사용하세요");
                                       return;
                                   }

                                   ConnectionString = connectionString;
                                   Console.WriteLine($"[INFO] Connection string set to: {connectionString}");
                               },
                               rootCommand.Children.OfType<Option<bool>>().First(), // "--app-version"
                               rootCommand.Children.OfType<Option<string>>().First()); // "--connection-string"

        return rootCommand;
    }
}
