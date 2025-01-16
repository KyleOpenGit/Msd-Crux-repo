namespace MSD.Crux.Core.Models;

/// <summary>
/// 제품의 한 주 동안 사출(inj)생산계획 응답 DTO
/// </summary>
/// <remarks>InjWeeklyQtyOfPartReqDto 요청에 대한 응답 DTO</remarks>
public class InjWeeklyPlanRspDto
{
    /// <summary>
    /// 제품ID. part 테이블의 레코드 id
    /// </summary>
    public string PartId { get; set; } = string.Empty;
    /// <summary>
    /// 주차 번호
    /// </summary>
    public int WeekNumber { get; set; }
    /// <summary>
    /// 해당 제품의 주차 전체 수량
    /// </summary>
    public int QtyWeekly { get; set; }
    /// <summary>
    /// 해당 주차 월~일 별 수량
    /// </summary>
    public List<DailyQty> DailyQtyList { get; set; }
}

/// <summary>
/// 하루 계획 수량
/// </summary>
public class DailyQty
{
    public DateTime Date { get; set; }
    public int Qty { get; set; }
}

/*
 {
  "PartId": "AAAA1",
  "WeekNumber": 3,
  "QtyWeekly": 1000,
  "DailyQtyList": [
    {
      "Date": "2025-01-15T00:00:00",
      "Qty": 0
    },
    {
      "Date": "2025-01-16T00:00:00",
      "Qty": 0
    },
    {
      "Date": "2025-01-17T00:00:00",
      "Qty": 0
    },
    ....
  ]
}

*/
