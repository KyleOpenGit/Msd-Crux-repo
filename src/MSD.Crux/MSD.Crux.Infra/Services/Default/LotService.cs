using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

public class LotService(ILotRepo _lotRepo, IPartRepo _partRepo) : ILotService
{
    public async Task<List<Lot?>> GetAllCompletedLotsAsync()
    {
        return await _lotRepo.GetAllCompletedLotsAsync();
    }

    public async Task<string> IssueNewLotIdAsync(LotIdIssueReqDto request)
    {
        // PartId 검증
        if (!await IsPartIdValidAsync(request.PartId))
        {
            throw new ArgumentException($"Invalid PartId: {request.PartId}는 존재하지 않습니다.");
        }

        string prefix = $"{request.PartId}-{request.Date:yyyyMMdd}-";

        // 가장 최신 순번 조회 후 새로운 Lot ID 생성
        int latestSequence = await _lotRepo.GetLatestSequenceOfIdAsync(request.PartId, request.Date);
        string newLotId = $"{prefix}{latestSequence + 1}";

        // 새로운 Lot 최소 정보 추가
        await _lotRepo.AddMinimalAsync(new Lot { Id = newLotId, PartId = request.PartId, LineId = request.LineId, IssuedTime = request.Date });

        return newLotId;
    }

    /// <summary>
    /// PartId 유효성 검증.
    /// </summary>
    /// <param name="partId">검증할 PartId</param>
    /// <returns>테이블 레코드 존재 여부</returns>
    private async Task<bool> IsPartIdValidAsync(string partId)
    {
        return await _partRepo.ExistsByIdAsync(partId);
    }
}
