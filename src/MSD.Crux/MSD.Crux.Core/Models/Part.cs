namespace MSD.Crux.Core.Models;

/// <summary>
/// 엔티티 클래스 - DB의 part 테이블 매핑
/// </summary>
public class Part
{
    /// <summary>
    /// 품목 ID (Primary Key)
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 품목 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 품목 유형 (2자리 숫자, 예: 01, 02)
    /// </summary>
    public short Type { get; set; }

    /// <summary>
    /// 차량 이름 (해당 품목이 사용되는 차량)
    /// </summary>
    public string? CarName { get; set; }

    /// <summary>
    /// 차량 모델
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 고객사 이름
    /// </summary>
    public string? Customer { get; set; }

    /// <summary>
    /// 활성 상태
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 비고
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// 차량 이미지 경로
    /// </summary>
    public string? CarImgPath { get; set; }

    /// <summary>
    /// 품목 이미지 경로
    /// </summary>
    public string? PartImgPath { get; set; }
}
