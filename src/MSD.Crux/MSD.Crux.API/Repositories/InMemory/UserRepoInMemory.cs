using System.Collections.Concurrent;
using MSD.Crux.Core.Models;

namespace MSD.Crux.API.Repositories.InMemory;

/// <summary>
/// User 테이블 인-메모리 레포지토리 구현체
/// </summary>
/// <remarks>주의: 싱글톤 DI 등록</remarks>
public class UserRepoInMemory : IUserRepo
{
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _currentId = 0;

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
    public Task<User?> GetByEmployeeNumberAsync(int employeeNumber)
    {
        var user = _users.Values.FirstOrDefault(u => u.EmployeeNumber == employeeNumber);
        return Task.FromResult(user);
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
        ValidateUser(user);
        user.Id = GenerateNewId();

        if (_users.ContainsKey(user.Id))
        {
            throw new InvalidOperationException($"유저 ID {user.Id}는 이미 존재합니다.");
        }

        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 기존 유저 정보를 업데이트.
    /// </summary>
    /// <param name="user">업데이트할 유저 정보를 가진 User 객체</param>
    /// <returns>비동기 작업 완료</returns>
    public Task UpdateAsync(User user)
    {
        ValidateUser(user);
        if (!_users.ContainsKey(user.Id))
        {
            throw new InvalidOperationException($"유저 ID {user.Id}는 존재하지 않습니다.");
        }

        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 특정 ID(PK)의 유저를 삭제.
    /// </summary>
    /// <param name="id">삭제할 유저의 ID</param>
    /// <returns>비동기 작업 완료</returns>
    public Task DeleteAsync(int id)
    {
        if (!_users.TryRemove(id, out _))
        {
            throw new InvalidOperationException($"유저 ID {id}는 존재하지 않습니다.");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// ID 자동 증가 메서드
    /// </summary>
    private int GenerateNewId()
    {
        return Interlocked.Increment(ref _currentId);
    }

    /// <summary>
    /// User 객체가 DB 칼럼 조건에 맞는지 검증하고, 맞지 않다면 예외를 던진다.
    /// </summary>
    /// <param name="user">검증할 User 객체</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private void ValidateUser(User user)
    {
        // login_id, login_pw, salt 유효성 검사: 모두 NULL이거나 모두 NOT NULL이어야 함
        bool allNull = string.IsNullOrEmpty(user.LoginId) && string.IsNullOrEmpty(user.LoginPw) && string.IsNullOrEmpty(user.Salt);
        bool allNotNull = !string.IsNullOrEmpty(user.LoginId) && !string.IsNullOrEmpty(user.LoginPw) && !string.IsNullOrEmpty(user.Salt);

        if (!allNull && !allNotNull)
        {
            throw new InvalidOperationException("LoginId, LoginPw, Salt는 모두 NULL이거나 모두 NOT NULL이어야 합니다.");
        }

        // Name이 비어 있으면 예외 발생
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            throw new ArgumentException("유저 이름은 필수입니다.", nameof(user.Name));
        }

        // EmployeeNumber가 기본값(0)이면 예외 발생
        if (user.EmployeeNumber <= 0)
        {
            throw new ArgumentException("사원번호는 1 이상의 값이어야 합니다.", nameof(user.EmployeeNumber));
        }
    }
}
