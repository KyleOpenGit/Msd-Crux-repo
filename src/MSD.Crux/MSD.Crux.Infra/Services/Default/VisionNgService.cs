using System.IO;
using Microsoft.Extensions.Configuration;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;
using MSD.Crux.Common;

namespace MSD.Crux.Infra.Services;

/// <summary>
/// vision_ng 테이블에 불량정보를 저장. 이미지를 appsettings.json에 지정된 경로에 저장 후 그 경로를 DB에 저장한다.
/// </summary>
public class VisionNgService : IVisionNgService
{
    private readonly IVisionNgRepo _visionNgRepo;
    /// <summary>
    /// 이미지 저장 베이스경로
    /// </summary>
    private readonly string _basePath;

    public VisionNgService(IVisionNgRepo visionNgRepo, IConfiguration configuration)
    {
        _visionNgRepo = visionNgRepo;

        // appsettings.json에서 이미지 저장 경로 가져오기
        _basePath = PathHelper.GetOrCreateDirectory(configuration["ImageStorage:BasePath"], "~/MSD.Crux.Host/images");
    }

    public async Task SaveVisionNgAsync(VisionNgReqDto request)
    {
        try
        {
            string filePath = await SaveImageAsync(_basePath, request.Img);

            var visionNg = new VisionNg { LotId = request.LotId, LineId = request.LineId, DateTime = request.DateTime, NgLabel = request.NgLabel.ToString(), NgImgPath = filePath };

            await _visionNgRepo.AddAsync(visionNg);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Vision NG 데이터를 저장하는 중 오류가 발생했습니다.", ex);
        }
    }

    private async Task<string> SaveImageAsync(string basePath, byte[] img)
    {
        try
        {
            string directoryPath = PathHelper.GetOrCreateDirectory(Path.Combine(basePath, DateTime.UtcNow.ToString("yyyy/MM/dd")));
            string fileName = $"{Guid.NewGuid()}.jpg";
            string filePath = Path.Combine(directoryPath, fileName);

            await File.WriteAllBytesAsync(filePath, img);
            return filePath;
        }
        catch (Exception ex)
        {
            throw new IOException("이미지를 저장하는 중 오류가 발생했습니다.", ex);
        }
    }
}
