using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

/// <summary>
/// injection_plan 테이블 엔티티 레포지토리 인터페이스
/// </summary>
public interface IInjectionPlanRepo
{
    /// <summary>
    /// 특정 Part ID와 날짜로 생산계획을 조회.
    /// </summary>
    /// <param name="partId">제품 ID</param>
    /// <param name="date">계획일</param>
    /// <returns>하나의 레코드</returns>
    Task<InjectionPlan?> GetByPartAndDateAsync(string partId, DateTime date);

    /// <summary>
    /// 특정 주차에 해당하는 모든 생산계획을 조회.
    /// </summary>
    /// <param name="weekNumber">ISO 주차 번호</param>
    /// <returns>InjectionPlan 객체의 리스트</returns>
    Task<IEnumerable<InjectionPlan>> GetByWeekAsync(int weekNumber);

    /// <summary>
    /// 특정 주차의 모든 레코드에서 Part ID로 묶고 날짜 기준으로 정렬.
    /// </summary>
    /// <param name="weekNumber">ISO 주차 번호</param>
    /// <returns>InjectionPlan 객체의 리스트</returns>
    Task<IEnumerable<IGrouping<string, InjectionPlan>>> GetGroupedByPartAndOrderedByDateAsync(int weekNumber);

    /// <summary>
    /// 특정 날짜의 모든 레코드에서 Part ID별 일일 수량 합계를 반환.
    /// </summary>
    /// <param name="date">특정 날짜</param>
    /// <returns>제품 ID와 일일 수량의 Dictionary</returns>
    Task<Dictionary<string, int>> GetDailyQuantitiesByPartAsync(DateTime date);

    /// <summary>
    /// 새로운 생산계획 레코드 추가.
    /// </summary>
    /// <param name="plan">추가할 레코드. InjectionPlan 객체</param>
    Task AddAsync(InjectionPlan plan);

    /// <summary>
    /// 기존 생산계획을 업데이트.
    /// </summary>
    /// <param name="plan">업데이트할 InjectionPlan 객체</param>
    Task UpdateAsync(InjectionPlan plan);

    /// <summary>
    /// 특정 Part ID와 날짜로 생산계획 삭제.
    /// </summary>
    /// <param name="partId">제품 ID</param>
    /// <param name="date">계획일</param>
    Task DeleteAsync(string partId, DateTime date);
}
