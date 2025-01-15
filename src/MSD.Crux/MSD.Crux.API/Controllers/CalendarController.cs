using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MSD.Crux.Common;

namespace MSD.Crux.API.Controllers;

[ApiController]
[Route("api/calendar")]
public class CalendarController : ControllerBase
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
}
