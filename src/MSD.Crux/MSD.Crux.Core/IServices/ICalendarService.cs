using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

/// <summary>
/// 주차번호와 관련된 캘린더 서비스 인터페이스
/// </summary>
public interface ICalendarService
{
    /// <summary>
    /// 특정 주차부터 지정된 개수만큼의 주차에 해당하는 날짜 목록을 반환.
    /// </summary>
    /// <param name="weekNumber">조회할 시작 주차 번호 (1~53)</param>
    /// <param name="count">조회할 주차 개수</param>
    /// <returns>조회한 주차별 날짜 목록</returns>
    /// <exception cref="ArgumentException">유효하지 않은 매개변수가 전달된 경우 발생.</exception>
    DateListsOfWeekNumbersRspDto GetDateLists(int weekNumber, int count);
}
