using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// vision_ng 테이블에 대한 레포지토리 인터페이스 구현체
/// </summary>
public class VisionNgRepoPsqlDb(IDbConnection _dbConnection) : IVisionNgRepo
{
    public async Task AddAsync(VisionNg visionNg)
    {
        const string query = @"
                INSERT INTO vision_ng (lot_id, line_id, date_time, ng_label, ng_img_path)
                VALUES (@LotId, @LineId, @DateTime, @NgLabel, @NgImgPath)";

        await _dbConnection.ExecuteAsync(query, new { visionNg.LotId, visionNg.LineId, visionNg.DateTime, visionNg.NgLabel, visionNg.NgImgPath });
    }

    public async Task<VisionNg?> GetByIdAsync(int id)
    {
        const string query = "SELECT * FROM vision_ng WHERE id = @Id";
        return await _dbConnection.QuerySingleOrDefaultAsync<VisionNg>(query, new { Id = id });
    }

    public async Task<IEnumerable<VisionNg>> GetAllAsync()
    {
        const string query = "SELECT * FROM vision_ng";
        return await _dbConnection.QueryAsync<VisionNg>(query);
    }

    public async Task UpdateAsync(VisionNg visionNg)
    {
        const string query = @"
            UPDATE vision_ng
            SET lot_id = @LotId,
                line_id = @LineId,
                date_time = @DateTime,
                ng_label = @NgLabel,
                ng_img_path = @NgImgPath
            WHERE id = @Id";
        await _dbConnection.ExecuteAsync(query, visionNg);
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM vision_ng WHERE id = @Id";
        await _dbConnection.ExecuteAsync(query, new { Id = id });
    }
}
