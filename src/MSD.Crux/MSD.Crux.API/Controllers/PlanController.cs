using Microsoft.AspNetCore.Mvc;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Controllers;

/// <summary>
/// Injection Plan 컨트롤러
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PlanController : ControllerBase
{
    private readonly IInjectionPlanService _injectionPlanService;

    public PlanController(IInjectionPlanService injectionPlanService)
    {
        _injectionPlanService = injectionPlanService;
    }

    /// <summary>
    /// 특정 주차의 생산계획 입력.
    /// </summary>
    [HttpPost("week")]
    public async Task<IActionResult> AddWeeklyPlan([FromBody] InjWeeklyQtyOfPartReqDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        InjWeeklyPlanRspDto? response = await _injectionPlanService.AddWeeklyPlanAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// 특정 주차 + 특정 제품의 모든 일일 생산 수량 수정.
    /// </summary>
    [HttpPut("week/{weekNumber}")]
    public async Task<IActionResult> UpdateDailyPlan(int weekNumber, [FromQuery] string partId, [FromBody] List<int> dailyQuantities)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _injectionPlanService.UpdateDailyPlanAsync(weekNumber, partId, dailyQuantities);
        return Ok(new { message = "수정 완료" });
    }

    /// <summary>
    /// 특정 날짜의 생산계획 조회.
    /// </summary>
    [HttpGet("day/{date:datetime}")]
    public async Task<IActionResult> GetDailyPlans(DateTime date)
    {
        Dictionary<string, int>? plans = await _injectionPlanService.GetDailyPlansAsync(date);
        return Ok(plans);
    }
}
