using System.Data;

namespace MSD.Crux.DevTool.Services;

public class TestingService(IDbConnection _dbConnection)
{
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
