using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

/// <summary>
/// Lot 조회, 발급에관한 서비스 인터페이스
/// </summary>
public interface ILotService
{
    /// <summary>
    /// 생산이 끝난 Lot를 조회
    /// </summary>
    /// <returns>조회된 로트정보 또는 null</returns>
    Task<List<Lot?>> GetAllCompletedLotsAsync();

    /// <summary>
    /// 새로운 Lot 번호 발급. 데이터 레포지토리에 요청된 part id, line id가 존재하는 경우에만 발급한다.
    /// </summary>
    /// <param name="request">Lot 번호 발급 요청 DTO</param>
    /// <returns>새로 발급된 Lot 번호</returns>
    Task<string> IssueNewLotIdAsync(LotIdIssueReqDto request);
}
