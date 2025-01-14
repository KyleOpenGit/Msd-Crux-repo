namespace MSD.Crux.Core.Models;

/// <summary>
/// 불량품 응답 DTO : /api/vision/ng/images?ids={쉼표구분id문자열}
/// </summary>
public class VisionNgImgRspDto
{
    /// <summary>
    /// vision_ng 테이블 레코드 id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 제품 아이디
    /// </summary>
    public string? PartId { get; set; }
    /// <summary>
    /// 불량검수 비전라인 id
    /// </summary>
    public string? LineId { get; set; }
    /// <summary>
    /// 불량 판정 시간
    /// </summary>
    public DateTime? DateTime { get; set; }
    /// <summary>
    /// 불량 구분 (AI Label)
    /// </summary>
    public string? NgLabel { get; set; }
    /// <summary>
    /// 불량사진 저장경로
    /// </summary>
    public string? NgImgPath { get; set; }
    /// <summary>
    /// 불량사진 이미지 데이터
    /// </summary>
    public string? NgImgBase64 { get; set; }
}
