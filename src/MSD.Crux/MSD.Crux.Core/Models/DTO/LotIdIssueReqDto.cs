namespace MSD.Crux.Core.Models;

/// <summary>
/// 새로운 Lot ID 발급 요청 DTO
/// </summary>
public class LotIdIssueReqDto
{
    /// <summary>
    /// 제품 ID  (AAAA1)
    /// </summary>
    public string PartId { get; set; } = string.Empty;
    /// <summary>
    /// 로트 발행 날짜 (yyyy-MM-dd)(2025-01-16)
    /// </summary>
    public DateTime Date { get; set; }
}
