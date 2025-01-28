using System.Globalization;

namespace MSD.Crux.Common;

/// <summary>
/// 주차 계산 캘린더
/// </summary>
public static class WCalendar
{
    /// <summary>
    /// 특정 날짜의 ISO 주차와 주차 내 날짜 목록을 반환
    /// </summary>
    /// <param name="date">기준 날짜</param>
    /// <returns>해당 주차의 날짜 목록</returns>
    public static DatesOfWeekNumber GetDatesOfISOWeek(DateTime date)
    {
        // ISO 주차 계산
        int weekNumber = ISOWeek.GetWeekOfYear(date);

        // 해당 주차의 첫 번째 요일 (ISO 기준 월요일) 계산
        DateTime firstDayOfWeek = date.AddDays(-(int)date.DayOfWeek + 1);
        if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            firstDayOfWeek = date.AddDays(-6); // ISO에서 일요일은 주의 마지막 날
        }

        // 주의 날짜 목록 생성
        List<DateTime> dates = new List<DateTime>();
        for (int i = 0; i < 7; i++)
        {
            dates.Add(firstDayOfWeek.AddDays(i));
        }

        // 결과 반환
        return new DatesOfWeekNumber { WeekNumber = weekNumber, Dates = dates };
    }
}
