using Microsoft.AspNetCore.Mvc;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Controllers.UserAndEmployee;

[ApiController]
[Route("api/login")]
public class UserLoginController(IUserLoginService _userLoginService) : ControllerBase
{
    /// <summary>
    /// 유저 로그인
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginReqDto loginInput)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            UserLoginRspDto? response = await _userLoginService.LoginAsync(loginInput.LoginId!, loginInput.LoginPw!);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }

    /// <summary>
    /// 사원번호에 해당하는 User의 로그인 Id/Pw를 등록.
    /// </summary>
    /// <param name="employeeNumber">사원번호</param>
    /// <param name="reqBody">로그인 ID와 PW raw data</param>
    [HttpPut("create-id/{employeeNumber:int}")]
    public async Task<IActionResult> CreateLoginIdAsync([FromRoute] int employeeNumber, [FromBody] UserLoginRegiReqDto reqBody)
    {
        try
        {
            UserLoginIdRegiRspDto? response = await _userLoginService.RegisterLoginIdAsync(employeeNumber, reqBody.LoginId, reqBody.LoginPw);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
        }
    }
}
