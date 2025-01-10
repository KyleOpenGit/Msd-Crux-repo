using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

/// <summary>
///  직원 등록 및 권한 신청 비즈니스 (디폴트) 로직 서비스에대한 계약
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// 새로운 직원 등록
    /// </summary>
    /// <param name="registeringEmployee">신규 등록할 직원 객체 </param>
    /// <returns>직원번호를 포함한 직원정보</returns>
    Task<EmployeeInfoRspDto> RegisterEmployeeAsync(Employee registeringEmployee);

    /// <summary>
    /// 전체 직원 조회
    /// </summary>
    /// <returns>직원번호를 포함한 모든 직원정보 목록</returns>
    Task<IEnumerable<EmployeeInfoRspDto>> GetAllEmployeesAsync();

    /// <summary>
    /// 직원정보로 특정 직원 조회
    /// </summary>
    /// <param name="employeeNumber">직원번호</param>
    /// <returns>employee 테이블에대한 엔티티 모델 객체</returns>
    Task<Employee?> GetEmployeeByNumberAsync(int employeeNumber);

    /// <summary>
    /// 직원 정보 업데이트 및 유저 권한 신청
    /// </summary>
    /// <param name="employee">employee 테이블에대한 엔티티 모델 객체<</param>
    Task UpdateEmployeeAsync(Employee employee);

    /// <summary>
    /// 직원번호로 특정 직원 찾아서 삭제
    /// </summary>
    /// <param name="employeeNumber">직원번호</param>
    Task DeleteEmployeeAsync(int employeeNumber);
}
