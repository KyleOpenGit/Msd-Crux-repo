namespace MSD.Crux.API.Models;

/// <summary>
/// 엔티티 클래스 - DB의 vision_ng 테이블 매핑.
/// AI Vision 검사 불량 판정품 사진 기록
/// </summary>
public class VisionNg
{
    /// <summary>
    /// PK - Auto Increment
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 생산 로트번호
    /// </summary>
    public string? LotId { get; set; }
    /// <summary>
    /// 품목코드
    /// </summary>
    public string? PartId { get; set; }
    /// <summary>
    /// 라인코드
    /// </summary>
    public string? LineId { get; set; }
    /// <summary>
    /// 불량 판정 시간
    /// </summary>
    public DateTime? DateTime { get; set; }
    /// <summary>
    /// 불량타입 레이블. hole, crack, scratch, dirty, not classified
    /// </summary>
    public string? NgLabel { get; set; }
    /// <summary>
    /// NG품 AI 분류 사진 저장 경로
    /// </summary>
    public string? NgImgPath { get; set; }
}
