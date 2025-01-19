using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

/// <summary>
/// Line 테이블에 대한 레포지토리 인터페이스
/// </summary>
public interface ILineRepo
{
    /// <summary>
    /// 특정 Line ID를 기반으로 Line 조회
    /// </summary>
    /// <param name="lineId">조회할 Line ID</param>
    /// <returns>조회된 Line 엔티티 또는 null</returns>
    Task<Line?> GetByIdAsync(string lineId);

    /// <summary>
    /// 새로운 Line 추가
    /// </summary>
    /// <param name="line">추가할 Line 엔티티</param>
    Task AddAsync(Line line);

    /// <summary>
    /// 특정 Line을 삭제
    /// </summary>
    /// <param name="lineId">삭제할 Line ID</param>
    Task DeleteAsync(string lineId);
}
