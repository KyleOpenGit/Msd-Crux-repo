using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

public interface IVisionNgService
{
    /// <summary>
    /// 이미지를 저장하고 Vision NG 데이터를 저장하고 DB에 기록한다.
    /// </summary>
    /// <param name="visionNgReqDto">Vision NG 요청 데이터</param>
    /// <returns>비동기 작업 완료</returns>
    Task<string> SaveVisionNgAsync(VisionNgReqDto visionNgReqDto);

    /// <summary>
    /// ID 목록으로 Vision NG 데이터를 조회.
    /// </summary>
    /// <param name="ids">조회할 ID 목록</param>
    /// <returns>조회된 Vision NG 데이터</returns>
    Task<List<VisionNgImgRspDto>> GetNgImgDataByIdsAsync(IEnumerable<int> ids);

    /// <summary>
    /// 여러 라인 ID로 최근 불량 이미지 경로를 조회
    /// </summary>
    /// <param name="request">라인 ID, Offset, Count 정보</param>
    /// <returns>이미지 저장경로를 포함한 라인 별 불량 목록</returns>
    Task<List<VisionNgImgPathRspDto>> GetRecentNgByLineIdAsync(VisionNgImgPathReqDto request);
}
