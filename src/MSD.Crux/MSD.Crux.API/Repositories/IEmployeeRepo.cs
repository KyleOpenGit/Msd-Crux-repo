using MSD.Crux.API.Models;

namespace MSD.Crux.API.Repositories;

/// <summary>
/// Employee 테이블에 대한 레포지토리 인터페이스
/// </summary>
public interface IEmployeeRepo
{
    /// <summary>
    /// 직원 번호로 특정 직원 조회
    /// </summary>
    Task<Employee?> GetByEmployeeNumberAsync(int employeeNumber);

    /// <summary>
    /// 모든 직원 조회
    /// </summary>
    Task<IEnumerable<Employee>> GetAllAsync();

    /// <summary>
    /// 새로운 직원 추가
    /// </summary>
    Task AddAsync(Employee employee);

    /// <summary>
    /// 기존 직원 정보 업데이트
    /// </summary>
    Task UpdateAsync(Employee employee);

    /// <summary>
    /// 직원 번호로 직원 삭제
    /// </summary>
    Task DeleteAsync(int employeeNumber);
}
