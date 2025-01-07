using System.Collections.Concurrent;
using MSD.Crux.API.Helpers;
using MSD.Crux.API.Models;

namespace MSD.Crux.API.Repositories.InMemory;

/// <summary>
/// Employee 테이블 In-Memory 레포지토리 구현체
/// </summary>
/// <remarks>주의: 싱글톤 DI 등록</remarks>
public class EmployeeRepoInMemory : IEmployeeRepo
{
    /// <summary>
    /// employee 테이블의 모든 레코드를 담는 메모리 저장소
    /// </summary>
    private readonly ConcurrentDictionary<int, Employee> _employees = new();

    /// <summary>
    /// 직원 존재 여부 확인
    /// </summary>
    /// <param name="employee">조회할 Employee 객체</param>
    /// <returns>존재여부</returns>
    public Task<bool> ExistsWithEmployeeNumberAsync(Employee employee)
    {
        int employeeNumber = GenericHelper.ConvertToEmployeeNumber(employee.Year, (short)employee.Gender, employee.Sequence);
        return Task.FromResult(_employees.ContainsKey(employeeNumber));
    }

    /// <summary>
    /// 사원번호로 직원 조회
    /// </summary>
    public Task<Employee?> GetByEmployeeNumberAsync(int employeeNumber)
    {
        _employees.TryGetValue(employeeNumber, out var employee);
        return Task.FromResult(employee);
    }

    /// <summary>
    /// 모든 직원 조회
    /// </summary>
    public Task<IEnumerable<Employee>> GetAllAsync()
    {
        return Task.FromResult(_employees.Values.AsEnumerable());
    }

    /// <summary>
    /// 새로운 직원 추가
    /// </summary>
    /// <param name="employee"></param>
    /// <returns></returns>
    public Task AddAsync(Employee employee)
    {
        int employeeNumber = GenericHelper.ConvertToEmployeeNumber(employee.Year, (short)employee.Gender, employee.Sequence);

        _employees[employeeNumber] = employee;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 기존 직원 정보 업데이트
    /// </summary>
    public Task UpdateAsync(Employee employee)
    {
        int employeeNumber = GenericHelper.ConvertToEmployeeNumber(employee.Year, (short)employee.Gender, employee.Sequence);

        if (!_employees.ContainsKey(employeeNumber))
        {
            throw new InvalidOperationException($"직원번호 {employeeNumber}는 존재하지 않습니다.");
        }

        _employees[employeeNumber] = employee;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 직원 번호로 직원 삭제
    /// </summary>
    public Task DeleteAsync(int employeeNumber)
    {
        if (!_employees.TryRemove(employeeNumber, out _))
        {
            throw new InvalidOperationException($"직원번호 {employeeNumber}는 존재하지 않습니다.");
        }

        return Task.CompletedTask;
    }
}
