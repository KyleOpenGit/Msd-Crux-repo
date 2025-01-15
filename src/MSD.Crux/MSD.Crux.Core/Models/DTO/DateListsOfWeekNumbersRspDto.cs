using MSD.Crux.Common;

namespace MSD.Crux.Core.Models;

/// <summary>
/// 주차당 날짜목록
/// </summary>
public class DateListsOfWeekNumbersRspDto
{
    public List<DatesOfWeekNumber> DatesOfWeekNumberList { get; set; }
}
