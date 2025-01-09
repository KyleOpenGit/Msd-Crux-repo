using MSD.Crux.API.Helpers;
using MSD.Crux.API.Repositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Services;

/// <summary>
/// 직원 등록 및 권한 신청 비즈니스 로직 서비스
/// </summary>
public class EmployeeService(IEmployeeRepo _employeeRepo)
{
    /// <summary>
    /// 새로운 직원 등록
    /// </summary>
    /// <param name="registeringEmployee">신규 등록할 직원 객체 </param>
    /// <returns>직원번호를 포함한 직원정보 객체</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<EmployeeInfoRspDto> RegisterEmployeeAsync(Employee registeringEmployee)
    {
        if (registeringEmployee == null)
        {
            throw new ArgumentNullException(nameof(registeringEmployee));
        }

        // 등록된 사원번호 중복 확인
        if (await _employeeRepo.ExistsWithEmployeeNumberAsync(registeringEmployee))
        {
            throw new InvalidOperationException("이미 등록된 사원번호입니다.");
        }

        // 신규 직원 추가 및 변환
        await _employeeRepo.AddAsync(registeringEmployee);
        Employee? registeredEmployee = registeringEmployee;
        return ConvertToEmployeeInfo(registeredEmployee);
    }

    /// <summary>
    /// 전체 직원 조회
    /// </summary>
    public async Task<IEnumerable<EmployeeInfoRspDto>> GetAllEmployeesAsync()
    {
        IEnumerable<Employee>? employees = await _employeeRepo.GetAllAsync();
        return employees.Select(ConvertToEmployeeInfo);
    }

    /// <summary>
    /// 특정 직원 조회
    /// </summary>
    public async Task<Employee?> GetEmployeeByNumberAsync(int employeeNumber)
    {
        return await _employeeRepo.GetByEmployeeNumberAsync(employeeNumber);
    }

    /// <summary>
    /// 직원 정보 업데이트 및 유저 권한 신청
    /// </summary>
    public async Task UpdateEmployeeAsync(Employee employee)
    {
        await _employeeRepo.UpdateAsync(employee);
    }

    /// <summary>
    /// 특정 직원 삭제
    /// </summary>
    public async Task DeleteEmployeeAsync(int employeeNumber)
    {
        await _employeeRepo.DeleteAsync(employeeNumber);
    }

    /// <summary>
    /// 직원 엔티티 객체를 적원정보 객체로 변환한다.
    /// </summary>
    /// <param name="employee">직원정보 엔티티 객체</param>
    /// <returns>편의성 적원정보 객체</returns>
    private EmployeeInfoRspDto ConvertToEmployeeInfo(Employee employee)
    {
        return new EmployeeInfoRspDto
        {
            EmployeeNumber = GenericHelper.ConvertToEmployeeNumber(employee.Year, employee.Gender, employee.Sequence),
            Sex = GenericHelper.ConvertToSex(employee.Gender),
            Name = employee.Name,
            Department = employee.Department,
            Shift = employee.Shift,
            Title = employee.Title,
            JoinDate = employee.JoinDate,
        };
    }
}
