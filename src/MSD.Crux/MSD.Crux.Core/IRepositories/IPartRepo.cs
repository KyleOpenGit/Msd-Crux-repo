using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

/// <summary>
/// Part 테이블에 대한 레포지토리 인터페이스
/// </summary>
public interface IPartRepo
{
    /// <summary>
    /// 특정 Part ID를 기반으로 Part 조회
    /// </summary>
    /// <param name="partId">조회할 Part ID</param>
    /// <returns>조회된 Part 엔티티 또는 null</returns>
    Task<Part?> GetByIdAsync(string partId);

    /// <summary>
    /// 새로운 Part 추가
    /// </summary>
    /// <param name="part">추가할 Part 엔티티</param>
    Task AddAsync(Part part);

    /// <summary>
    /// 특정 Part를 삭제
    /// </summary>
    /// <param name="partId">삭제할 Part ID</param>
    Task DeleteAsync(string partId);
}
