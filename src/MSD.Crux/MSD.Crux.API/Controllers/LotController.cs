using Microsoft.AspNetCore.Mvc;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Controllers;

/// <summary>
/// 공정 단위인 lot번호의 생성, 조회 ,업데이트 API
/// </summary>
[ApiController]
[Route("api")]
public class LotController(ILotService _lotService) : ControllerBase
{
    /// <summary>
    /// 생산이 완료된 lot를 조회
    /// </summary>
    /// <returns>lotlist 또는 null</returns>
    [HttpGet("lots/completed")]
    public async Task<IActionResult> GetAllCompletedLots()
    {
        IEnumerable<Lot?>? lots = await _lotService.GetAllCompletedLotsAsync();
        return Ok(lots);
    }

    /// <summary>
    /// 새로운 Lot ID 발급
    /// </summary>
    /// <param name="request">Lot ID 발급 요청 DTO</param>
    /// <returns>발급된 Lot ID</returns>
    [HttpPost("lot/issue-new")]
    public async Task<IActionResult> IssueNewLotId([FromBody] LotIdIssueReqDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string newLotId = await _lotService.IssueNewLotIdAsync(request);
        return Ok(new { LotId = newLotId });
    }
}
