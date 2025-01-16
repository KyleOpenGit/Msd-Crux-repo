namespace MSD.Crux.Core.Models;

/// <summary>
/// injection_plan 테이블 모델 클래스
/// </summary>
public class InjectionPlan
{
    /// <summary>
    /// part 테이블의 id
    /// </summary>
    public string PartId { get; set; }
    /// <summary>
    /// 계획일
    /// </summary>
    public DateTime Date { get; set; }
    /// <summary>
    /// 요일
    /// </summary>
    public string Day { get; set; }
    /// <summary>
    /// 일일 계획수량
    /// </summary>
    public int QtyDaily { get; set; }
    /// <summary>
    /// 주 계획수량
    /// </summary>
    public int IsoWeek { get; set; }
    /// <summary>
    /// 주 계획수량
    /// </summary>
    public int QtyWeekly { get; set; }

}
