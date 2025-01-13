using System.IO;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

public class VisionNgService(IVisionNgRepo _visionNgRepo) : IVisionNgService
{
    public async Task SaveVisionNgAsync(VisionNgReqDto request)
    {
        //TODO:  저장 위치를 홈 폴더에? 실행파일 폴더에?
        string? directoryPath = Path.Combine("wwwroot", "images", DateTime.UtcNow.ToString("yyyy/MM/dd"));
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 이미지 저장
        string? fileName = $"{Guid.NewGuid()}.jpg";
        string? filePath = Path.Combine(directoryPath, fileName);
        await File.WriteAllBytesAsync(filePath, request.Img);

        var visionNg = new VisionNg { LotId = request.LotId, LineId = request.LineId, DateTime = request.DateTime, NgLabel = request.NgLabel.ToString(), NgImgPath = filePath };

        await _visionNgRepo.AddAsync(visionNg);
    }
}
