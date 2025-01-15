namespace MSD.Crux.Common;

/// <summary>
/// 특정 ISO 주차에 속한 날짜목록
/// </summary>
public class DatesOfWeekNumber
{
    /// <summary>
    /// ISO 주차
    /// </summary>
    public int WeekNumber { get; set; }
    /// <summary>
    /// 주차에 속한 날짜. 월~일 순서대로
    /// </summary>
    public List<DateTime> Dates { get; set; }
}
/*
{
"week-number": 1,
[2025-01-01, 2025-01-02,2025-01-03,2025-01-04,]
}
*/
