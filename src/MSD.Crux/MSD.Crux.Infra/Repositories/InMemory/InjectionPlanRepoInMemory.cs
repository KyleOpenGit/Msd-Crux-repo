using System.Collections.Concurrent;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// injection_plan 테이블 In-Memory 레포지토리 구현체
/// </summary>
public class InjectionPlanRepoInMemory : IInjectionPlanRepo
{
    private readonly ConcurrentDictionary<(string, DateTime), InjectionPlan> _plans = new();

    public Task<InjectionPlan?> GetByPartAndDateAsync(string partId, DateTime date)
    {
        _plans.TryGetValue((partId, date), out var plan);
        return Task.FromResult(plan);
    }

    public Task<IEnumerable<InjectionPlan>> GetByWeekAsync(int weekNumber)
    {
        var plans = _plans.Values.Where(p => p.WeekNumber == weekNumber);
        return Task.FromResult(plans.AsEnumerable());
    }

    public Task AddAsync(InjectionPlan plan)
    {
        if (!_plans.TryAdd((plan.PartId, plan.Date), plan))
        {
            throw new InvalidOperationException($"Plan for part {plan.PartId} on {plan.Date:yyyy-MM-dd} already exists.");
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(InjectionPlan plan)
    {
        if (!_plans.ContainsKey((plan.PartId, plan.Date)))
        {
            throw new InvalidOperationException($"Plan for part {plan.PartId} on {plan.Date:yyyy-MM-dd} does not exist.");
        }

        _plans[(plan.PartId, plan.Date)] = plan;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string partId, DateTime date)
    {
        if (!_plans.TryRemove((partId, date), out _))
        {
            throw new InvalidOperationException($"Plan for part {partId} on {date:yyyy-MM-dd} does not exist.");
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<IGrouping<string, InjectionPlan>>> GetGroupedByPartAndOrderedByDateAsync(int weekNumber)
    {
        var grouped = _plans.Values.Where(p => p.WeekNumber == weekNumber).OrderBy(p => p.Date).GroupBy(p => p.PartId);

        return Task.FromResult(grouped.AsEnumerable());
    }

    public Task<Dictionary<string, int>> GetDailyQuantitiesByPartAsync(DateTime date)
    {
        var result = _plans.Values.Where(p => p.Date.Date == date.Date).GroupBy(p => p.PartId).ToDictionary(g => g.Key, g => g.Sum(p => p.QtyDaily));

        return Task.FromResult(result);
    }
}
