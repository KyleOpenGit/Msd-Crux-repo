using System.Collections.Concurrent;
using MSD.Crux.API.Models;

namespace MSD.Crux.API.Repositories.InMemory;

/// <summary>
/// User 테이블 인-메모리 레포지토리 구현체
/// </summary>
/// <remarks>주의: 싱글톤 DI 등록</remarks>
public class UserRepoInMemory : IUserRepo
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private readonly ConcurrentDictionary<string, int> _employeeNumbers = new(); // EmployeeNumber → UserId 매핑
    private int _nextId = 1;

    /// <summary>
    /// ID(PK)를 기준으로 특정 유저를 조회.
    /// </summary>
    /// <param name="id">유저 ID(PK)</param>
    /// <returns>조회된 유저 정보 또는 null</returns>
    public Task<User?> GetByIdAsync(int id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    /// <summary>
    /// Login ID를 기준으로 특정 유저를 조회.
    /// </summary>
    /// <param name="loginId">유저 로그인 ID</param>
    /// <returns>조회된 유저 정보 또는 null</returns>
    public Task<User?> GetByLoginIdAsync(string loginId)
    {
        var user = _users.Values.FirstOrDefault(u => u.LoginId == loginId);
        return Task.FromResult(user);
    }

    /// <summary>
    /// Employee Number를 기준으로 특정 유저를 조회.
    /// </summary>
    /// <param name="employeeNumber">사원번호</param>
    /// <returns>조회된 유저 정보 또는 null</returns>
    public Task<User?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        if (_employeeNumbers.TryGetValue(employeeNumber, out int userId))
        {
            _users.TryGetValue(userId, out var user);
            return Task.FromResult(user);
        }

        return Task.FromResult<User?>(null);
    }

    /// <summary>
    /// 모든 유저를 조회.
    /// </summary>
    /// <returns>유저 목록</returns>
    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult(_users.Values.AsEnumerable());
    }

    /// <summary>
    /// 새로운 유저를 추가.
    /// </summary>
    /// <param name="user">추가할 유저 정보를 가진 User 객체</param>
    /// <returns>비동기 작업 완료</returns>
    public Task AddAsync(User user)
    {
        user.Id = _nextId++;
        _users[user.Id] = user;

        if (!string.IsNullOrEmpty(user.EmployeeNumber))
        {
            _employeeNumbers[user.EmployeeNumber] = user.Id;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 기존 유저 정보를 업데이트.
    /// </summary>
    /// <param name="user">업데이트할 유저 정보를 가진 User 객체</param>
    /// <returns>비동기 작업 완료</returns>
    public Task UpdateAsync(User user)
    {
        if (_users.ContainsKey(user.Id))
        {
            _users[user.Id] = user;

            if (!string.IsNullOrEmpty(user.EmployeeNumber))
            {
                _employeeNumbers[user.EmployeeNumber] = user.Id;
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 특정 ID(PK)의 유저를 삭제.
    /// </summary>
    /// <param name="id">삭제할 유저의 ID</param>
    /// <returns>비동기 작업 완료</returns>
    public Task DeleteAsync(int id)
    {
        if (_users.TryRemove(id, out var user))
        {
            if (!string.IsNullOrEmpty(user.EmployeeNumber))
            {
                _employeeNumbers.TryRemove(user.EmployeeNumber, out _);
            }
        }

        return Task.CompletedTask;
    }
}
