namespace MSD.Crux.Core.Models;

/// <summary>
/// 엔티티 클래스 - DB의 line 테이블 매핑
/// </summary>
public class Line
{
    /// <summary>
    /// 라인 ID (Primary Key)
    /// </summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// 라인 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 라인 유형 (예: Injection, Vision)
    /// </summary>
    public string LineType { get; set; } = string.Empty;
    /// <summary>
    /// 라인 셋업 날짜
    /// </summary>
    public DateTime SetupDate { get; set; }
    /// <summary>
    /// 라인 제조 업체
    /// </summary>
    public string? Maker { get; set; }
    /// <summary>
    /// 비고
    /// </summary>
    public string? Note { get; set; }
}
