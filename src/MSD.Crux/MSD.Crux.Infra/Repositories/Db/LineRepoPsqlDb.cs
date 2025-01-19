using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// Line 테이블에 대한 PostgreSQL DB 구현체
/// </summary>
/// <param name="_dbConnection">DIC에서 주입되는 Db 커넥션 객체</param>
public class LineRepoPsqlDb(IDbConnection _dbConnection) : ILineRepo
{
    public async Task<Line?> GetByIdAsync(string lineId)
    {
        const string query = "SELECT * FROM line WHERE id = @LineId";
        return await _dbConnection.QuerySingleOrDefaultAsync<Line>(query, new { LineId = lineId });
    }

    public async Task AddAsync(Line line)
    {
        const string query = @"
                INSERT INTO line (id, name, line_type, setup_date, maker, note)
                VALUES (@Id, @Name, @LineType, @SetupDate, @Maker, @Note)";
        await _dbConnection.ExecuteAsync(query, line);
    }

    public async Task DeleteAsync(string lineId)
    {
        const string query = "DELETE FROM line WHERE id = @LineId";
        await _dbConnection.ExecuteAsync(query, new { LineId = lineId });
    }
}
