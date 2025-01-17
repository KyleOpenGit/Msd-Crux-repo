using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

/// <summary>
/// Injection Plan 서비스 인터페이스
/// </summary>
public interface IInjectionPlanService
{
    /// <summary>
    /// 특정 주차의 제품 생산계획 입력.
    /// </summary>
    /// <param name="request">주차 생산계획 요청 DTO</param>
    /// <returns>주차 생산계획 응답 DTO</returns>
    Task<InjWeeklyPlanRspDto> AddWeeklyPlanAsync(InjWeeklyQtyOfPartReqDto request);

    /// <summary>
    /// 특정 주차와 제품의 일일 생산계획 수정.
    /// </summary>
    /// <param name="weekNumber">주차 번호</param>
    /// <param name="partId">제품 ID</param>
    /// <param name="dailyQuantities">일일 수량 배열</param>
    Task UpdateDailyPlanAsync(int weekNumber, string partId, List<int> dailyQuantities);

    /// <summary>
    /// 특정 날짜의 모든 제품 일일 생산계획 조회.
    /// </summary>
    /// <param name="date">날짜</param>
    /// <returns>제품별 목표 수량</returns>
    Task<Dictionary<string, int>> GetDailyPlansAsync(DateTime date);

    /// <summary>
    /// 특정 주차의 모든 제품별 주간 생산계획 조회
    /// </summary>
    /// <param name="weekNumber">주차 번호</param>
    /// <returns>주간 생산계획 응답 DTO 리스트</returns>
    Task<List<InjWeeklyPlanRspDto>> GetWeeklyPlansAsync(int weekNumber);
}
