using Microsoft.AspNetCore.Mvc;
using MSD.Crux.API.Helpers;
using MSD.Crux.API.Models;
using MSD.Crux.API.Services;

namespace MSD.Crux.API.Controllers;

[ApiController]
[Route("api/hr/employees")]
public class EmployeeController(EmployeeService _employeeService) : ControllerBase
{
    /// <summary>
    /// 새로운 직원 등록
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterEmployee([FromBody] Employee reqBody)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        EmployeeInfoRspDto? registeredEmployeeInfo = await _employeeService.RegisterEmployeeAsync(reqBody);

        // 201 Created 응답
        return CreatedAtAction(nameof(GetEmployeeByNumber), new { employeeNumber = registeredEmployeeInfo.EmployeeNumber }, registeredEmployeeInfo);
    }

    /// <summary>
    /// 전체 직원 조회
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        IEnumerable<EmployeeInfoRspDto>? employeeInfos = await _employeeService.GetAllEmployeesAsync();
        return Ok(employeeInfos);
    }

    /// <summary>
    /// 특정 직원 조회
    /// </summary>
    [HttpGet("{employeeNumber}")]
    public async Task<IActionResult> GetEmployeeByNumber(int employeeNumber)
    {
        var employee = await _employeeService.GetEmployeeByNumberAsync(employeeNumber);
        if (employee == null)
            return NotFound($"사원번호 {employeeNumber}에 해당하는 직원이 없습니다.");

        return Ok(employee);
    }

    /// <summary>
    /// 직원 정보 업데이트 및 권한 신청
    /// </summary>
    [HttpPut("{employeeNumber}")]
    public async Task<IActionResult> UpdateEmployee(int employeeNumber, [FromBody] Employee reqBody)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _employeeService.UpdateEmployeeAsync(reqBody);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// 특정 직원 삭제
    /// </summary>
    [HttpDelete("{employeeNumber}")]
    public async Task<IActionResult> DeleteEmployee(int employeeNumber)
    {
        try
        {
            await _employeeService.DeleteEmployeeAsync(employeeNumber);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
