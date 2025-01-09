using System.Data;
using Dapper;
using MSD.Crux.Core.Models;
using MSD.Crux.Core.Repositories;

namespace MSD.Crux.API.Repositories.Psql;

/// <summary>
/// User 테이블 PostgreSQL 레포지토리 구현체
/// </summary>
public class UserRepoPsqlDb : IUserRepo
{
    private readonly IDbConnection _dbConnection;

    /// <summary>
    /// (생성자). Service Provider를 통해 DIC에 등록된  IDbConnection 객체를 주입해준다
    /// </summary>
    /// <param name="dbConnection">DI가 주입해주는 Npgsql로 만든 IDbConnection 객체 </param>
    public UserRepoPsqlDb(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string query = "SELECT * FROM public.user WHERE id = @Id";
        return await _dbConnection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task<User?> GetByLoginIdAsync(string loginId)
    {
        const string query = "SELECT * FROM public.user WHERE login_id = @LoginId";
        return await _dbConnection.QuerySingleOrDefaultAsync<User>(query, new { LoginId = loginId });
    }

    public async Task<User?> GetByEmployeeNumberAsync(int employeeNumber)
    {
        const string query = "SELECT * FROM public.user WHERE employee_number = @EmployeeNumber";
        return await _dbConnection.QuerySingleOrDefaultAsync<User>(query, new { EmployeeNumber = employeeNumber });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string query = "SELECT * FROM public.user";
        return await _dbConnection.QueryAsync<User>(query);
    }

    public async Task AddAsync(User user)
    {
        const string query = @"INSERT INTO public.user (employee_number, login_id, login_pw, salt, name, roles)
                                VALUES (@EmployeeNumber, @LoginId, @LoginPw, @Salt, @Name, @Roles)
                                RETURNING id";
        user.Id = await _dbConnection.ExecuteScalarAsync<int>(query, user);
    }

    public async Task UpdateAsync(User user)
    {
        const string query = @"UPDATE public.user
                                SET login_id = @LoginId, login_pw = @LoginPw, salt = @Salt, name = @Name, roles = @Roles
                                WHERE id = @Id";
        await _dbConnection.ExecuteAsync(query, user);
    }

    public async Task DeleteAsync(int id)
    {
        const string query = "DELETE FROM public.user WHERE id = @Id";
        await _dbConnection.ExecuteAsync(query, new { Id = id });
    }
}
