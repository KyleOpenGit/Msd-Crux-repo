using System.IO;
using Microsoft.Extensions.Configuration;
using MSD.Crux.Common;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services;

/// <summary>
/// vision_ng 테이블에 불량정보를 저장. 이미지를 appsettings.json에 지정된 경로에 저장 후 그 경로를 DB에 저장한다.
/// </summary>
public class VisionNgService : IVisionNgService
{
    private readonly IVisionNgRepo _visionNgRepo;
    private readonly ILotRepo _lotRepo;
    /// <summary>
    /// 이미지 저장 베이스경로. appsettings 구성파일 "ImageStorage:BasePath" 키의 값
    /// </summary>
    private readonly string _basePath;

    public VisionNgService(IVisionNgRepo visionNgRepo, ILotRepo lotRepo, IConfiguration configuration)
    {
        _visionNgRepo = visionNgRepo;
        _lotRepo = lotRepo;

        // appsettings.json에서 이미지 저장 경로 가져오기
        _basePath = PathHelper.GetOrCreateDirectory(configuration["ImageStorage:BasePath"], "~/MSD.Crux.Host/images");
    }

    public async Task SaveVisionNgAsync(VisionNgReqDto visionNgReqDto)
    {
        try
        {
            // Lot 정보를 조회하여 part_id 가져오기
            var lot = await _lotRepo.GetByIdAsync(visionNgReqDto.LotId);
            if (lot == null)
            {
                throw new InvalidOperationException($"LotId '{visionNgReqDto.LotId}'에 해당하는 데이터를 찾을 수 없습니다.");
            }

            string filePath = await SaveImageAsync(_basePath, visionNgReqDto.Img);

            // Vision NG 데이터 생성
            var visionNg = new VisionNg
            {
                LotId = visionNgReqDto.LotId,
                PartId = lot.PartId,
                LineId = visionNgReqDto.LineId,
                DateTime = visionNgReqDto.DateTime,
                NgLabel = visionNgReqDto.NgLabel.ToString(),
                NgImgPath = filePath
            };

            await _visionNgRepo.AddAsync(visionNg);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Vision NG 데이터를 저장하는 중 오류가 발생했습니다.", ex);
        }
    }

    /// <summary>
    /// 제공된 이미지 byte 데이터를 년월일 경로와 GUID로된 이름의 jpg 파일로 저장하고 그 경로를 반환한다ㅏ.
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="img">저장할 이미지 데이터</param>
    /// <returns>이미지 저장 절대경로</returns>
    /// <exception cref="IOException">이미지 저장 I/O 바운드 작업 오류시 예외</exception>
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
