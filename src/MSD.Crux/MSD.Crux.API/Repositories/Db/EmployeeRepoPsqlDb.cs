using System.Data;
using Dapper;
using MSD.Crux.API.Helpers;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Repositories.PsqlDb;

/// <summary>
/// Employee 테이블 PostgreSQL 레포지토리 구현체
/// </summary>
public class EmployeeRepoPsqlDb : IEmployeeRepo
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// (생성자). Service Provider를 통해 DIC에 등록된  IDbConnection 객체를 주입해준다
    /// </summary>
    /// <param name="dbConnection">DI가 주입해주는 Npgsql로 만든 IDbConnection 객체 </param>
    public EmployeeRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<bool> ExistsWithEmployeeNumberAsync(Employee employee)
    {
        int employeeNumber = GenericHelper.ConvertToEmployeeNumber(employee.Year, employee.Gender, employee.Sequence);
        const string query = "SELECT COUNT(1) FROM employee WHERE year = @Year AND gender = @Gender AND sequence = @Sequence";
        return await _dbConnection.ExecuteScalarAsync<bool>(query, new { employee.Year, Gender = (int)employee.Gender, employee.Sequence });
    }

    public async Task<Employee?> GetByEmployeeNumberAsync(int employeeNumber)
    {
        const string query = "SELECT * FROM employee WHERE (year * 1000000 + gender * 10000 + sequence) = @EmployeeNumber";
        return await _dbConnection.QueryFirstOrDefaultAsync<Employee>(query, new { EmployeeNumber = employeeNumber });
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        const string query = "SELECT * FROM employee";
        return await _dbConnection.QueryAsync<Employee>(query);
    }

    public async Task AddAsync(Employee employee)
    {
        const string query = @"INSERT INTO employee (year, gender, sequence, name, department, shift, title, join_date, apply_user_roles, photo)
                                VALUES (@Year, @Gender, @Sequence, @Name, @Department, @Shift, @Title, @JoinDate, @ApplyUserRoles, @Photo)";
        await _dbConnection.ExecuteAsync(query,
                                         new
                                         {
                                             employee.Year,
                                             Gender = (int)employee.Gender,
                                             employee.Sequence,
                                             employee.Name,
                                             employee.Department,
                                             employee.Shift,
                                             employee.Title,
                                             employee.JoinDate,
                                             employee.ApplyUserRoles,
                                             employee.Photo
                                         });
    }

    public async Task UpdateAsync(Employee employee)
    {
        const string query = @"UPDATE employee
                                SET name = @Name, department = @Department, shift = @Shift, title = @Title, join_date = @JoinDate, apply_user_roles = @ApplyUserRoles, photo = @Photo
                                WHERE year = @Year AND gender = @Gender AND sequence = @Sequence";
        int rowsAffected = await _dbConnection.ExecuteAsync(query,
                                                            new
                                                            {
                                                                employee.Year,
                                                                Gender = (int)employee.Gender,
                                                                employee.Sequence,
                                                                employee.Name,
                                                                employee.Department,
                                                                employee.Shift,
                                                                employee.Title,
                                                                employee.JoinDate,
                                                                employee.ApplyUserRoles,
                                                                employee.Photo
                                                            });

        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"직원번호 {GenericHelper.ConvertToEmployeeNumber(employee.Year, employee.Gender, employee.Sequence)}는 존재하지 않습니다.");
        }
    }

    public async Task DeleteAsync(int employeeNumber)
    {
        const string query = @"DELETE FROM employee WHERE (year * 1000000 + gender * 10000 + sequence) = @EmployeeNumber";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, new { EmployeeNumber = employeeNumber });
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"직원번호 {employeeNumber}는 존재하지 않습니다.");
        }
    }
}
