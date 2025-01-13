using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// User 테이블 PostgreSQL 레포지토리 구현체
/// </summary>
public class VisionCumRepoPsqlDb : IVisionCumRepo
{
    private readonly IDbConnection _dbConnection;

    public VisionCumRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<int> AddVisionCumAsync(VisionCum visionCum)
    {
        const string query = @"INSERT INTO public.vision_cum (line_id, time, lot_id, shift, employee_number, total)
                    VALUES (@LineId, @Time, @LotId, @Shift, @EmployeeNumber, @Total);";
        return await _dbConnection.ExecuteAsync(query, visionCum);
    }
}
