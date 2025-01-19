using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// Part 테이블에 대한 In-Memory 레포지토리 구현체
/// </summary>
/// <remarks>주의: 싱글톤 주입</remarks>
public class PartRepoInMemory : IPartRepo
{
    private readonly ConcurrentDictionary<string, Part> _parts = new();

    public Task<bool> ExistsByIdAsync(string partId)
    {
        return Task.FromResult(_parts.ContainsKey(partId));
    }

    public Task AddAsync(Part part)
    {
        if (_parts.ContainsKey(part.Id))
        {
            throw new InvalidOperationException($"Part ID {part.Id}는 이미 존재합니다.");
        }

        _parts[part.Id] = part;
        return Task.CompletedTask;
    }

    public Task<Part?> GetByIdAsync(string partId)
    {
        _parts.TryGetValue(partId, out var part);
        return Task.FromResult(part);
    }

    public Task DeleteAsync(string partId)
    {
        if (!_parts.TryRemove(partId, out _))
        {
            throw new InvalidOperationException($"Part ID {partId}는 존재하지 않습니다.");
        }

        return Task.CompletedTask;
    }
}
