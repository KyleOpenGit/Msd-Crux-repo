using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

public class LotService(ILotRepo _lotRepo) : ILotService
{
    public async Task<List<Lot?>> GetAllCompletedLotsAsync()
    {
        return await _lotRepo.GetAllCompletedLotsAsync();
    }

    public async Task<string> IssueNewLotIdAsync(LotIdIssueReqDto request)
    {
        string prefix = $"{request.PartId}-{request.Date:yyyyMMdd}-";

        // 가장 최신 순번 조회 후 새로운 Lot ID 생성
        int latestSequence = await _lotRepo.GetLatestSequenceOfIdAsync(request.PartId, (request.Date));

        string newLotId = $"{prefix}{latestSequence + 1}";

        // 새로운 Lot 최소 정보 추가
        await _lotRepo.AddMinimalAsync(new Lot { Id = newLotId, PartId = request.PartId, LineId = request.LineId, IssuedTime = request.Date });

        return newLotId;
    }
}
