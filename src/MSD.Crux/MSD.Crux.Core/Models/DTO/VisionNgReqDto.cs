namespace MSD.Crux.Core.Models;

public class VisionNgReqDto
{
    public string LotId { get; set; }
    public string LineId { get; set; }
    public DateTime DateTime { get; set; }
    /// <summary>
    /// 불량 타입 (AI 트레이닝 Label)
    /// </summary>
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
    NotClassified
}

/*
{
    "LotId": "AAAA1-20250131-1",
    "LineId": "vi01",
    "DateTime": "2025-01-13T14:00:00Z",
    "NgLabel": "Scratch",
    "Img": "Base64EncodedImageData"
}
*/
