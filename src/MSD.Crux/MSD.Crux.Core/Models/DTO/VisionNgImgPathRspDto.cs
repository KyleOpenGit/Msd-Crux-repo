namespace MSD.Crux.Core.Models;

/// <summary>
/// 응답 DTO.
///<remarks>POST: /api/vision/ng/images</remarks>
/// </summary>
public class VisionNgImgPathRspDto
{
    /// <summary>
    /// 라인 ID
    /// </summary>
    public string LineId { get; set; } = string.Empty;
    /// <summary>
    /// 해당 라인의 불량 데이터 목록
    /// </summary>
    public List<VisionNgImgRspDto> VisionNgImages { get; set; } = new();
}
