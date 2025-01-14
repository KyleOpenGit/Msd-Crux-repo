using Microsoft.AspNetCore.Mvc;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisionController : ControllerBase
{
    private readonly IVisionNgService _visionNgService;

    public VisionController(IVisionNgService visionNgService)
    {
        _visionNgService = visionNgService;
    }

    /// <summary>
    /// 비전검사 NG품에대한 정보와 사진을 저장한다. 사진은 Byte[] 배열로 받는다.
    /// </summary>
    /// <param name="request">불량품 정보와 이미지</param>
    [HttpPost("ng")]
    public async Task<IActionResult> SaveVisionNg([FromBody] VisionNgReqDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _visionNgService.SaveVisionNgAsync(request);
            return Ok(new { message = "Vision NG 데이터 저장 성공" });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
        catch (IOException ex)
        {
            return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "알 수 없는 오류가 발생했습니다.", details = ex.Message });
        }
    }
}
