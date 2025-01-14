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
        _basePath = GetOrCreateDirectory(configuration["ImageStorage:BasePath"], "~/MSD.Crux.Host/images");
    }

    public async Task SaveVisionNgAsync(VisionNgReqDto request)
    {
        string directoryPath = GetOrCreateDirectory(Path.Combine(_basePath, DateTime.UtcNow.ToString("yyyy/MM/dd")));

        // 이미지 저장
        string? fileName = $"{Guid.NewGuid()}.jpg";
        string? filePath = Path.Combine(directoryPath, fileName);
        await File.WriteAllBytesAsync(filePath, request.Img);

        var visionNg = new VisionNg { LotId = request.LotId, LineId = request.LineId, DateTime = request.DateTime, NgLabel = request.NgLabel.ToString(), NgImgPath = filePath };

        await _visionNgRepo.AddAsync(visionNg);
    }

    /// <summary>
    /// 설정된 경로를 읽고 절대 경로로 변환하며, 디렉토리를 생성한다.
    /// </summary>
    /// <param name="path">설정된 경로</param>
    /// <param name="defaultPath">기본값 경로</param>
    /// <returns>절대 경로</returns>
    private static string GetOrCreateDirectory(string? path, string defaultPath = "")
    {
        string resolvedPath = PathHelper.ToAbsolutePath(path ?? defaultPath);

        if (!Directory.Exists(resolvedPath))
        {
            Directory.CreateDirectory(resolvedPath);
        }
        return resolvedPath;
    }
}
