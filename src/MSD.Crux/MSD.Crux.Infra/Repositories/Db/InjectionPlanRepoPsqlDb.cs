using System.Data;
using Dapper;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// injection_plan 테이블 레포지토리 PostgreSQL 구현체
/// </summary>
public class InjectionPlanRepoPsqlDb : IInjectionPlanRepo
{
    private readonly IDbConnection _dbConnection;

    public InjectionPlanRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<InjectionPlan?> GetByPartAndDateAsync(string partId, DateTime date)
    {
        const string query = "SELECT * FROM injection_plan WHERE part_id = @PartId AND date = @Date";
        return await _dbConnection.QueryFirstOrDefaultAsync<InjectionPlan>(query, new { PartId = partId, Date = date });
    }

    public async Task<IEnumerable<InjectionPlan>> GetByWeekAsync(int weekNumber)
    {
        const string query = "SELECT * FROM injection_plan WHERE week_number = @WeekNumber";
        return await _dbConnection.QueryAsync<InjectionPlan>(query, new { WeekNumber = weekNumber });
    }

    public async Task<IEnumerable<IGrouping<string, InjectionPlan>>> GetGroupedByPartAndOrderedByDateAsync(int weekNumber)
    {
        const string query = @"SELECT * FROM injection_plan WHERE week_number = @WeekNumber ORDER BY part_id, date";
        var plans = await _dbConnection.QueryAsync<InjectionPlan>(query, new { WeekNumber = weekNumber });
        return plans.GroupBy(p => p.PartId);
    }

    public async Task<Dictionary<string, int>> GetDailyQuantitiesByPartAsync(DateTime date)
    {
        const string query = @"SELECT part_id, SUM(qty_daily) AS total_qty
                               FROM injection_plan WHERE date = @Date GROUP BY part_id";
        var results = await _dbConnection.QueryAsync(query, new { Date = date });

        return results.ToDictionary(row => (string)row.part_id, row => (int)row.total_qty);
    }

    public async Task AddAsync(InjectionPlan plan)
    {
        const string query = @"INSERT INTO injection_plan (part_id, date, day, qty_daily, week_number, qty_weekly)
                               VALUES (@PartId, @Date, @Day, @QtyDaily, @IsoWeek, @QtyWeekly)";
        await _dbConnection.ExecuteAsync(query, new { plan.PartId, plan.Date, plan.Day, plan.QtyDaily, IsoWeek = plan.IsoWeek, plan.QtyWeekly });
    }

    public async Task UpdateAsync(InjectionPlan plan)
    {
        const string query = @"UPDATE injection_plan
                               SET day = @Day, qty_daily = @QtyDaily, week_number = @IsoWeek, qty_weekly = @QtyWeekly
                               WHERE part_id = @PartId AND date = @Date";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, new { plan.PartId, plan.Date, plan.Day, plan.QtyDaily, IsoWeek = plan.IsoWeek, plan.QtyWeekly });

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"Injection plan for part {plan.PartId} on {plan.Date:yyyy-MM-dd} does not exist.");
        }
    }

    public async Task DeleteAsync(string partId, DateTime date)
    {
        const string query = "DELETE FROM injection_plan WHERE part_id = @PartId AND date = @Date";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, new { PartId = partId, Date = date });

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"Injection plan for part {partId} on {date:yyyy-MM-dd} does not exist.");
        }
    }
}
