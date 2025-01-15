using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MSD.Crux.Common;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Controllers;

/// <summary>
/// 주차 계산 및 주차에속한 날짜 찾기 엔드포인트
/// </summary>
[ApiController]
[Route("api/calendar")]
public class CalendarController(ICalendarService _calendarService) : ControllerBase
{
    [HttpGet("week-number/{date}")]
    public ActionResult<DatesOfWeekNumber> GetWeekNumber(DateTime date)
    {
        if (date < new DateTime(2000, 1, 1) || date > new DateTime(2100, 12, 31))
        {
            return BadRequest("날짜는 2000-01-01과 2100-12-31 사이여야 합니다.");
        }

        DatesOfWeekNumber? dates = WCalendar.GetDatesOfISOWeek(date);

        return Ok(dates);
    }

    /// <summary>
    /// 특정 주차부터 지정된 갯수만큼의 주차 날짜 목록 조회
    /// </summary>
    /// <param name="weekNumber">조회 시작 주차 번호</param>
    /// <param name="count">조회할 주차 수</param>
    /// <returns>주차별 날짜 목록</returns>
    [HttpGet("date-list/{week-number:int}/{count:int}")]
    public ActionResult<DateListsOfWeekNumbersRspDto> GetDateList([FromRoute(Name = "week-number")] int weekNumber, [FromRoute(Name = "count")] int count)
    {
        try
        {
            DateListsOfWeekNumbersRspDto? result = _calendarService.GetDateLists(weekNumber, count);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
