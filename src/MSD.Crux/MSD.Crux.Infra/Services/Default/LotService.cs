using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

/// <summary>
/// Lot 조회, 발급에관한 서비스 인터페이스 디폴트 구현체
/// </summary>
/// <param name="_lotRepo">DIC로부터 주입받는 ILotRepo 구현체 객체</param>
/// <param name="_partRepo">DIC로부터 주입받는 IPartRepo 구현체 객체</param>
/// <param name="_lineRepo">DIC로부터 주입받는 ILineRepo 구현체 객체</param>
public class LotService(ILotRepo _lotRepo, IPartRepo _partRepo, ILineRepo _lineRepo) : ILotService
{
    public async Task<List<Lot?>> GetAllCompletedLotsAsync()
    {
        return await _lotRepo.GetAllCompletedLotsAsync();
    }

    public async Task<string> IssueNewLotIdAsync(LotIdIssueReqDto request)
    {
        await ValidateIdsAsync(request.PartId, request.LineId);

        string prefix = $"{request.PartId}-{request.Date:yyyyMMdd}-";

        // 가장 최신 순번 조회 후 새로운 Lot ID 생성
        int latestSequence = await _lotRepo.GetLatestSequenceOfIdAsync(request.PartId, request.Date);
        string newLotId = $"{prefix}{latestSequence + 1}";

        // 새로운 Lot 최소 정보 추가
        await _lotRepo.AddMinimalAsync(new Lot { Id = newLotId, PartId = request.PartId, LineId = request.LineId, IssuedTime = request.Date });

        return newLotId;
    }

    /// <summary>
    /// PartId와 LineId 유효성 검증
    /// </summary>
    /// <param name="partId">검증할 PartId</param>
    /// <param name="lineId">검증할 LineId</param>
    private async Task ValidateIdsAsync(string partId, string lineId)
    {
        if (await _partRepo.GetByIdAsync(partId) == null)
            throw new ArgumentException($"Invalid PartId: {partId}는 존재하지 않습니다.");
        if (await _lineRepo.GetByIdAsync(lineId) == null)
            throw new ArgumentException($"Invalid LineId: {lineId}는 존재하지 않습니다.");
    }
}
