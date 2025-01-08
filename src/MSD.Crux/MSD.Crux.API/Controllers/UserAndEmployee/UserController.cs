using Microsoft.AspNetCore.Mvc;
using MSD.Crux.API.Models;
using MSD.Crux.API.Services;

namespace MSD.Crux.API.Controllers;

/// <summary>
/// 공장 시스템 사용 유저 등록 및 권한 승인 API
/// </summary>
[ApiController]
[Route("api/users")]
public class UserController(UserService _userService) : ControllerBase
{
    /// <summary>
    /// 새로운 유저 등록 == 시스템 사용 권한 승인
    /// </summary>
    /// <param name="reqBody">사용신청 직원 정보 및 신청하는 권한</param>
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegiReqDto reqBody)
    {
        try
        {
            UserRegiRspDto? response = await _userService.RegisterUserAsync(reqBody);
            return CreatedAtAction(nameof(GetUserById), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// 전체 유저 정보 조회
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        IEnumerable<UserInfoRspDto>? users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// User 레코드 ID로 유저 조회
    /// </summary>
    /// <param name="id">DB 레코드 id 칼럼 값</param>
    [HttpGet("by-id/{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        UserInfoRspDto? userInfo = await _userService.GetUserByIdAsync(id);
        if (userInfo == null)
        {
            return NotFound($"ID {id}에 해당하는 유저가 없습니다.");
        }

        return Ok(userInfo);
    }

    /// <summary>
    /// id에 해당하는 User 정보 수정
    /// </summary>
    [HttpPut("by-id/{id:int}")]
    public async Task<IActionResult> UpdateUserById([FromRoute] int id, [FromBody] UserUpdateReqDto updateReqDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _userService.UpdateUserByIdAsync(id, updateReqDto);
            return NoContent(); // 성공 시 204 No Content 반환
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// 직원번호로 유저 조회
    /// </summary>
    [HttpGet("by-number/{employeeNumber:int}")]
    public async Task<IActionResult> GetUserByEmployeeNumber(int employeeNumber)
    {
        UserInfoRspDto? userInfo = await _userService.GetUserByEmployeeNumberAsync(employeeNumber);
        if (userInfo == null)
        {
            return NotFound($"사원번호 {employeeNumber}에 해당하는 유저가 없습니다.");
        }

        return Ok(userInfo);
    }

    /// <summary>
    /// 직원번호에 해당하는 User 정보 수정
    /// </summary>
    [HttpPut("by-number/{employeeNumber:int}")]
    public async Task<IActionResult> UpdateUserByEmployeeNumber([FromRoute] int employeeNumber, [FromBody] UserUpdateReqDto updateReqDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _userService.UpdateUserByEmployeeNumberAsync(employeeNumber, updateReqDto);
            return NoContent(); // 성공 시 204 No Content 반환
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    /// <summary>
    ///  User 등록 및 권한 신청한 employee 목록
    /// </summary>
    /// <returns></returns>
    [HttpGet("applications")]
    public async Task<IActionResult> GetUserRoleApplications()
    {
        IEnumerable<UserRoleApplyRspDto>? applications = await _userService.GetUserRoleApplicationsAsync();
        return Ok(applications);
    }
}
