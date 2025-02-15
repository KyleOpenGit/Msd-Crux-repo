﻿using Microsoft.AspNetCore.Mvc;
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
    /// <returns>lot list 또는 null</returns>
    [HttpGet("lots/completed")]
    public async Task<IActionResult> GetAllCompletedLots()
    {
        try
        {
            IEnumerable<Lot?>? lots = await _lotService.GetAllCompletedLotsAsync();
            return Ok(lots);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    /// <summary>
    /// 새로운 Lot ID 발급
    /// </summary>
    /// <param name="request">Lot ID 발급 요청 DTO</param>
    /// <returns>발급된 Lot ID</returns>
    [HttpPost("lot/issue-new")]
    public async Task<IActionResult> IssueNewLotId([FromBody] LotIdIssueReqDto request)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.PartId) || string.IsNullOrWhiteSpace(request.LineId) || request.Date == default)
        {
            return BadRequest("PartId, LineId,  Date 모두 필수이며 값형식이 맞아야합니다.");
        }

        try
        {
            string newLotId = await _lotService.IssueNewLotIdAsync(request);
            return Ok(new { LotId = newLotId });
        }
        catch (ArgumentException ex)
        {
            // 유효하지 않은 입력에 대한 처리
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // 예기치 않은 오류 처리
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
