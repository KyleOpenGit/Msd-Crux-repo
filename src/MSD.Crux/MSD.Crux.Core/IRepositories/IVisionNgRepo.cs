using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

/// <summary>
/// vision_ng 테이블에 대한 레포지토리 인터페이스
/// </summary>
public interface IVisionNgRepo
{
    /// <summary>
    /// 새로운 Vision NG 데이터를 추가
    /// </summary>
    /// <param name="visionNg">추가할 Vision NG 엔티티</param>
    /// <returns>비동기 작업 완료</returns>
    Task AddAsync(VisionNg visionNg);

    /// <summary>
    /// Vision NG 데이터를 ID로 조회
    /// </summary>
    /// <param name="id">조회할 Vision NG 데이터의 ID</param>
    /// <returns>조회된 Vision NG 데이터 또는 null</returns>
    Task<VisionNg?> GetByIdAsync(int id);

    /// <summary>
    /// 모든 Vision NG 데이터를 조회
    /// </summary>
    /// <returns>Vision NG 데이터 리스트</returns>
    Task<IEnumerable<VisionNg>> GetAllAsync();

    /// <summary>
    /// 기존 Vision NG 데이터를 업데이트
    /// </summary>
    /// <param name="visionNg">업데이트할 Vision NG 엔티티</param>
    /// <returns>비동기 작업 완료</returns>
    Task UpdateAsync(VisionNg visionNg);

    /// <summary>
    /// Vision NG 데이터를 ID로 삭제
    /// </summary>
    /// <param name="id">삭제할 Vision NG 데이터의 ID</param>
    /// <returns>비동기 작업 완료</returns>
    Task DeleteAsync(int id);
}
