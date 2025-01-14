using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

public class VisionNgRepoPsqlDb(IDbConnection _dbConnection) : IVisionNgRepo
{
    public async Task AddAsync(VisionNg visionNg)
    {
        const string query = @"
                INSERT INTO vision_ng (lot_id, line_id, date_time, ng_label, ng_img_path)
                VALUES (@LotId, @LineId, @DateTime, @NgLabel, @NgImgPath)";

        await _dbConnection.ExecuteAsync(query, new { visionNg.LotId, visionNg.LineId, visionNg.DateTime, visionNg.NgLabel, visionNg.NgImgPath });
    }
}
