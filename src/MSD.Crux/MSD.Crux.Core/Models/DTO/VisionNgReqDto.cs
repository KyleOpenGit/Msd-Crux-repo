using System.Text.Json.Serialization;

namespace MSD.Crux.Core.Models;

/// <summary>
/// 요청 DTO. 이미지저장
/// </summary>
/// <remarks>POST: /api/vision/ng</remarks>
public class VisionNgReqDto
{
    public string LotId { get; set; }
    public string LineId { get; set; }
    public DateTime DateTime { get; set; }
    /// <summary>
    /// 불량 타입 (AI 트레이닝 Label)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NgType NgLabel { get; set; }
    /// <summary>
    ///  바이트 배열 이미지 데이터
    /// </summary>
    public byte[] Img { get; set; }
}

public enum NgType
{
    Hole,
    Crack,
    Scratch,
    Dirty,
    Mixed,
    NotClassified
}

/*
{
    "LotId": "AAAA1-20250131-1",
    "LineId": "vi01",
    "DateTime": "2025-01-13T14:00:00Z",
    "NgLabel": 0,
    "Img": "Base64EncodedImageData"
}
*/
