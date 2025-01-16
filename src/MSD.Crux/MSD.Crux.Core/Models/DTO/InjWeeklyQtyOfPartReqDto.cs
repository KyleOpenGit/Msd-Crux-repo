
namespace MSD.Crux.Core.Models;
/// <summary>
/// 한 제품에대한 한 주차 사출(inj)전체 계획 수량 Insert 요청 DTO
/// </summary>
public class InjWeeklyQtyOfPartReqDto
{
    /// <summary>
    /// part 테이블의 레코드 id
    /// </summary>
    public string PartId { get; set; } = string.Empty;
    /// <summary>
    /// 주차 번호
    /// </summary>
    public int WeekNumber { get; set; }
    /// <summary>
    /// 주차에대한 전체 계획 수량
    /// </summary>
    public int QtyWeekly { get; set; }
}
