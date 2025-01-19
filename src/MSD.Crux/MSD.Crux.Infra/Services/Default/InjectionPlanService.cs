using System.Globalization;
using MSD.Crux.Common;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

/// <summary>
/// Injection Plan 서비스 구현체
/// </summary>
public class InjectionPlanService(IInjectionPlanRepo _injectionPlanRepo) : IInjectionPlanService
{
    public async Task<InjWeeklyPlanRspDto> AddWeeklyPlanAsync(InjWeeklyQtyOfPartReqDto request)
    {
        // 요청된 주차 번호를 기반으로 첫 번째 날짜 계산
        DateTime firstDayOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1); // 현재 연도 기준
        DateTime startOfWeek = ISOWeek.ToDateTime(firstDayOfYear.Year, request.WeekNumber, DayOfWeek.Monday);

        // 주차에 속한 날짜 계산
        var weekInfo = WCalendar.GetDatesOfISOWeek(startOfWeek);
        var dates = weekInfo.Dates;

        // 계획 데이터 생성 및 저장
        foreach (var date in dates)
        {
            InjectionPlan plan = new()
            {
                PartId = request.PartId,
                Date = date,
                Day = date.ToString("ddd", CultureInfo.InvariantCulture),
                QtyDaily = 0,
                WeekNumber = request.WeekNumber,
                QtyWeekly = request.QtyWeekly
            };
            await _injectionPlanRepo.AddAsync(plan);
        }

        // 응답 DTO 생성
        return new InjWeeklyPlanRspDto
        {
            PartId = request.PartId,
            WeekNumber = request.WeekNumber,
            QtyWeekly = request.QtyWeekly,
            DailyQtyList = dates.Select(d => new DailyQty { Date = d, Qty = 0 }).ToList()
        };
    }

    public async Task UpdateDailyPlanAsync(int weekNumber, string partId, List<int> dailyQuantities)
    {
        IEnumerable<InjectionPlan>? plans = await _injectionPlanRepo.GetByWeekAsync(weekNumber);
        List<InjectionPlan>? filteredPlans = plans.Where(p => p.PartId == partId).OrderBy(p => p.Date).ToList();

        if (filteredPlans.Count != dailyQuantities.Count)
        {
            throw new ArgumentException("일일 수량과 주차 내 날짜 수가 일치하지 않습니다.");
        }

        for (int i = 0; i < filteredPlans.Count; i++)
        {
            InjectionPlan? plan = filteredPlans[i];
            plan.QtyDaily = dailyQuantities[i];
            await _injectionPlanRepo.UpdateAsync(plan);
        }
    }

    public async Task<Dictionary<string, int>> GetDailyPlansAsync(DateTime date)
    {
        return await _injectionPlanRepo.GetDailyQuantitiesByPartAsync(date);
    }

    public async Task<List<InjWeeklyPlanRspDto>> GetWeeklyPlansAsync(int weekNumber)
    {
        // 주어진 주차 번호에 대한 모든 생산계획 조회
        IEnumerable<InjectionPlan> weeklyPlanByWeekNumber = await _injectionPlanRepo.GetByWeekAsync(weekNumber);

        // 제품별로 그룹화
        IEnumerable<IGrouping<string, InjectionPlan>>? groupedPlans = weeklyPlanByWeekNumber.GroupBy(p => p.PartId);

        // 응답 DTO 리스트 생성
        List<InjWeeklyPlanRspDto>? weeklyPartsPlan = groupedPlans.Select(group =>
                                                                {
                                                                    InjectionPlan? firstPlan = group.First();
                                                                    return new InjWeeklyPlanRspDto
                                                                    {
                                                                        PartId = firstPlan.PartId,
                                                                        WeekNumber = firstPlan.WeekNumber,
                                                                        QtyWeekly = firstPlan.QtyWeekly,
                                                                        DailyQtyList = group.Select(p => new DailyQty { Date = p.Date, Qty = p.QtyDaily }).OrderBy(d => d.Date).ToList()
                                                                    };
                                                                }).ToList();

        return weeklyPartsPlan;
    }
}
