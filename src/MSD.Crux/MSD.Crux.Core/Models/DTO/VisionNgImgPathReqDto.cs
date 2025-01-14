namespace MSD.Crux.Core.Models;

/// <summary>
/// 요청 DTO. 여러 라인id에대한 최근불량 이미지 path 목록
/// </summary>
/// <code>
/// {
/// "LineIds": ["vi01", "vi02"],
/// "Offset": 0,
/// "Count": 5
/// }
/// </code>
/// <remarks>POST: /api/vision/ng/images</remarks>
public class VisionNgImgPathReqDto
{
    /// <summary>
    /// 라인 ID 목록
    /// </summary>
    public List<string> LineIds { get; set; } = new();
    /// <summary>
    /// 데이터 조회 시작 위치 (Offset)
    /// </summary>
    public int Offset { get; set; }
    /// <summary>
    /// 조회할 데이터 개수 (Limit)
    /// </summary>
    public int Count { get; set; }
}
