using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

/// <summary>
/// Lot 테이블 관련 서비스 구현체
/// </summary>
public interface ILotService
{
    /// <summary>
    /// 생산이 끝난 Lot를 조회
    /// </summary>
    /// <returns>조회된 로트정보 또는 null</returns>
    Task<List<Lot?>> GetAllCompletedLotsAsync();

    /// <summary>
    /// 새로운 Lot ID 발급
    /// </summary>
    /// <param name="request">Lot ID 발급 요청 DTO</param>
    /// <returns>새로 발급된 Lot ID</returns>
    Task<string> IssueNewLotIdAsync(LotIdIssueReqDto request);
}
