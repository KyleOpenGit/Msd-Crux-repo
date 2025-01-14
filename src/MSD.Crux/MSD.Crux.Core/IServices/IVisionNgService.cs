using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

public interface IVisionNgService
{
    /// <summary>
    /// 이미지를 저장하고 Vision NG 데이터를 저장하고 DB에 기록한다.
    /// </summary>
    /// <param name="visionNgReqDto">Vision NG 요청 데이터</param>
    /// <returns>비동기 작업 완료</returns>
    Task SaveVisionNgAsync(VisionNgReqDto visionNgReqDto);
}
