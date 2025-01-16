using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

public class LotRepoPsqlDb : ILotRepo
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// (생성자). Service Provider를 통해 DIC에 등록된  IDbConnection 객체를 주입해준다
    /// </summary>
    /// <param name="dbConnection">DI가 주입해주는 Npgsql로 만든 IDbConnection 객체 </param>
    public LotRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<List<Lot?>> GetAllCompletedLotsAsync()
    {
        const string query = "SELECT * FROM lot WHERE injection_end IS NOT NULL;";
        var result = await _dbConnection.QueryAsync<Lot?>(query);
        return result.ToList();
    }

    public async Task<Lot?> GetByIdAsync(string id)
    {
        const string query = "SELECT * FROM lot WHERE id = @Id";
        return await _dbConnection.QuerySingleOrDefaultAsync<Lot>(query, new { Id = id });
    }

    public async Task<int> GetLatestSequenceOfIdAsync(string partId, DateTime date)
    {
        const string query = @"
            SELECT COALESCE(MAX(SUBSTRING(id FROM '-(\d+)$')::INTEGER), 0)
            FROM lot
            WHERE id LIKE @Prefix || '%'";
        string prefix = $"{partId}-{date:yyyyMMdd}-";
        return await _dbConnection.ExecuteScalarAsync<int>(query, new { Prefix = prefix });
    }

    public async Task AddAsync(Lot lot)
    {
        const string query = @"
            INSERT INTO lot (id, part_id, line_id, issued_time, qty, completed_qty, vision_line_ids, injection_start, injection_end, injection_worker, vision_start, vision_end, vision_worker, supplier, note)
            VALUES (@Id, @PartId, @LineId, @IssuedTime, @Qty, @CompletedQty, @VisionLineIds, @InjectionStart, @InjectionEnd, @InjectionWorker, @VisionStart, @VisionEnd, @VisionWorker, @Supplier, @Note)";
        await _dbConnection.ExecuteAsync(query, lot);
    }

    public async Task AddMinimalAsync(Lot lot)
    {
        const string query = @"
            INSERT INTO lot (id, part_id, issued_time)
            VALUES (@Id, @PartId, @IssuedTime)";
        await _dbConnection.ExecuteAsync(query, new { lot.Id, lot.PartId, lot.IssuedTime });
    }

    public async Task UpdateAsync(Lot lot)
    {
        const string query = @"
            UPDATE lot
            SET part_id = @PartId,
                line_id = @LineId,
                issued_time = @IssuedTime,
                qty = @Qty,
                completed_qty = @CompletedQty,
                vision_line_ids = @VisionLineIds,
                injection_start = @InjectionStart,
                injection_end = @InjectionEnd,
                injection_worker = @InjectionWorker,
                vision_start = @VisionStart,
                vision_end = @VisionEnd,
                vision_worker = @VisionWorker,
                supplier = @Supplier,
                note = @Note
            WHERE id = @Id";
        await _dbConnection.ExecuteAsync(query, lot);
    }

    public async Task DeleteAsync(string id)
    {
        const string query = "DELETE FROM lot WHERE id = @Id";
        await _dbConnection.ExecuteAsync(query, new { Id = id });
    }
}
