using System.Collections.Concurrent;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// vision_ng 테이블에 대한 인-메모리 레포지토리 구현체
/// </summary>
/// <remarks>주의: 싱글톤 DI 등록</remarks>
public class VisionNgRepoInMemory : IVisionNgRepo
{
    private readonly ConcurrentDictionary<int, VisionNg> _visionNgData = new();
    private int _currentId = 0;

    public Task AddAsync(VisionNg visionNg)
    {
        visionNg.Id = GenerateNewId();
        if (!_visionNgData.TryAdd(visionNg.Id, visionNg))
        {
            throw new InvalidOperationException($"ID {visionNg.Id}는 이미 존재합니다.");
        }

        return Task.CompletedTask;
    }

    public Task<VisionNg?> GetByIdAsync(int id)
    {
        _visionNgData.TryGetValue(id, out var visionNg);
        return Task.FromResult(visionNg);
    }

    public Task<IEnumerable<VisionNg>> GetAllAsync()
    {
        return Task.FromResult(_visionNgData.Values.AsEnumerable());
    }

    public Task UpdateAsync(VisionNg visionNg)
    {
        if (!_visionNgData.ContainsKey(visionNg.Id))
        {
            throw new InvalidOperationException($"ID {visionNg.Id}는 존재하지 않습니다.");
        }

        _visionNgData[visionNg.Id] = visionNg;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        if (!_visionNgData.TryRemove(id, out _))
        {
            throw new InvalidOperationException($"ID {id}는 존재하지 않습니다.");
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<VisionNg>> GetByLineIdAsync(string lineId, int offset, int count)
    {
        var filtered = _visionNgData.Values.Where(ng => ng.LineId == lineId).OrderByDescending(ng => ng.DateTime) // 최신순 정렬
                                    .Skip(offset).Take(count);

        return Task.FromResult(filtered);
    }

    private int GenerateNewId()
    {
        return Interlocked.Increment(ref _currentId);
    }
}
