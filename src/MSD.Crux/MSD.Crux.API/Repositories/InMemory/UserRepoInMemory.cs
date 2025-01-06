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
    private int _nextId = 1;

    public Task<User?> GetByIdAsync(int id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByLoginIdAsync(string loginId)
    {
        var user = _users.Values.FirstOrDefault(u => u.LoginId == loginId);
        return Task.FromResult(user);
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return Task.FromResult(_users.Values.AsEnumerable());
    }

    public Task AddAsync(User user)
    {
        user.Id = _nextId++;
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        if (_users.ContainsKey(user.Id))
        {
            _users[user.Id] = user;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _users.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
