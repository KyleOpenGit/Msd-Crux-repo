using System.Collections.Concurrent;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// Line 테이블에 대한 In-Memory 레포지토리 구현체
/// </summary>
public class LineRepoInMemory : ILineRepo
{
    private readonly ConcurrentDictionary<string, Line> _lines = new();

    public Task<Line?> GetByIdAsync(string lineId)
    {
        _lines.TryGetValue(lineId, out var line);
        return Task.FromResult(line);
    }

    public Task AddAsync(Line line)
    {
        if (_lines.ContainsKey(line.Id))
        {
            throw new InvalidOperationException($"Line ID {line.Id}는 이미 존재합니다.");
        }

        _lines[line.Id] = line;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string lineId)
    {
        if (!_lines.TryRemove(lineId, out _))
        {
            throw new InvalidOperationException($"Line ID {lineId}는 존재하지 않습니다.");
        }

        return Task.CompletedTask;
    }
}
