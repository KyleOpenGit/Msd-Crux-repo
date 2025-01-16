using System.Collections.Concurrent;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Repositories;

/// <summary>
/// Lot 테이블 인-메모리 레포지토리 구현체
/// </summary>
/// <remarks>주의: 싱글톤 DI 등록</remarks>
public class LotRepoInMemory : ILotRepo
{
    private readonly ConcurrentDictionary<string, Lot> _lots = new();

    /// <summary>
    /// 생산이 완료된 Lot를 조회.
    /// </summary>
    /// <returns>생산 완료된 Lot 리스트</returns>
    public Task<List<Lot?>> GetAllCompletedLotsAsync()
    {
        var completedLots = _lots.Values.Where(lot => lot.InjectionEnd != default).Cast<Lot?>().ToList();

        return Task.FromResult(completedLots);
    }

    /// <summary>
    /// Lot ID를 기준으로 특정 Lot 조회.
    /// </summary>
    /// <param name="id">Lot ID</param>
    /// <returns>조회된 Lot 정보 또는 null</returns>
    public Task<Lot?> GetByIdAsync(string id)
    {
        _lots.TryGetValue(id, out var lot);
        return Task.FromResult(lot);
    }

    public Task<int> GetLatestSequenceOfIdAsync(string partId, DateTime date)
    {
        // Prefix 생성
        string prefix = $"{partId}-{date:yyyyMMdd}-";

        // 해당 Prefix로 시작하는 Lot ID를 필터링
        var matchingLots = _lots.Keys.Where(key => key.StartsWith(prefix)).Select(key =>
                                                                                  {
                                                                                      // ID의 마지막 순번 추출
                                                                                      string[] parts = key.Split('-');
                                                                                      return int.TryParse(parts.LastOrDefault(), out int sequence) ? sequence : 0;
                                                                                  });

        // 최대 순번 반환
        int maxSequence = matchingLots.Any() ? matchingLots.Max() : 0;
        return Task.FromResult(maxSequence);
    }

    /// <summary>
    /// 새로운 Lot를 추가.
    /// </summary>
    /// <param name="lot">추가할 Lot 정보</param>
    /// <returns>비동기 작업 완료</returns>
    public Task AddAsync(Lot lot)
    {
        ValidateLot(lot);

        if (_lots.ContainsKey(lot.Id))
        {
            throw new InvalidOperationException($"Lot ID {lot.Id}는 이미 존재합니다.");
        }

        _lots[lot.Id] = lot;
        return Task.CompletedTask;
    }

    public Task AddMinimalAsync(Lot lot)
    {
        // 최소 필드만 유효성 검사
        if (string.IsNullOrWhiteSpace(lot.Id))
        {
            throw new ArgumentException("Lot ID는 필수입니다.", nameof(lot.Id));
        }

        if (string.IsNullOrWhiteSpace(lot.PartId))
        {
            throw new ArgumentException("Part ID는 필수입니다.", nameof(lot.PartId));
        }

        if (_lots.ContainsKey(lot.Id))
        {
            throw new InvalidOperationException($"Lot ID {lot.Id}는 이미 존재합니다.");
        }

        _lots[lot.Id] = new Lot { Id = lot.Id, PartId = lot.PartId, IssuedTime = lot.IssuedTime };

        return Task.CompletedTask;
    }

    /// <summary>
    /// 기존 Lot를 업데이트.
    /// </summary>
    /// <param name="lot">업데이트할 Lot 정보</param>
    /// <returns>비동기 작업 완료</returns>
    public Task UpdateAsync(Lot lot)
    {
        ValidateLot(lot);

        if (!_lots.ContainsKey(lot.Id))
        {
            throw new InvalidOperationException($"Lot ID {lot.Id}는 존재하지 않습니다.");
        }

        _lots[lot.Id] = lot;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Lot ID를 기준으로 Lot를 삭제.
    /// </summary>
    /// <param name="id">삭제할 Lot ID</param>
    /// <returns>비동기 작업 완료</returns>
    public Task DeleteAsync(string id)
    {
        if (!_lots.TryRemove(id, out _))
        {
            throw new InvalidOperationException($"Lot ID {id}는 존재하지 않습니다.");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Lot 객체의 유효성을 검증.
    /// </summary>
    /// <param name="lot">검증할 Lot 객체</param>
    /// <exception cref="ArgumentException"></exception>
    private void ValidateLot(Lot lot)
    {
        // Lot ID 검증
        if (string.IsNullOrWhiteSpace(lot.Id))
        {
            throw new ArgumentException("Lot ID는 필수입니다.", nameof(lot.Id));
        }

        // PartId 검증
        if (string.IsNullOrWhiteSpace(lot.PartId))
        {
            throw new ArgumentException("Part ID는 필수입니다.", nameof(lot.PartId));
        }

        // InjectionStart가 InjectionEnd보다 늦을 수 없음
        if (lot.InjectionStart > lot.InjectionEnd)
        {
            throw new ArgumentException("InjectionStart는 InjectionEnd보다 빠르거나 같아야 합니다.", nameof(lot.InjectionStart));
        }
    }
}
