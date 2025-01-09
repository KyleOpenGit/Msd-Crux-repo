namespace MSD.Crux.API.Models;

/// <summary>
/// 엔티티 클래스 - DB의 injection_cum 테이블 매핑.
/// 누적생산량 수집 시계열데이터.
/// </summary>
/// <remarks>line_id + time 복합키(PK)</remarks>
public class InjectionCum
{
    /// <summary>
    /// 라인코드
    /// </summary>
    public string LineId { get; set; } = string.Empty;
    /// <summary>
    /// TIMESTAMP
    /// </summary>
    public DateTime Time { get; set; }
    /// <summary>
    /// 생산 로트번호
    /// </summary>
    public string? LotId { get; set; }
    /// <summary>
    /// 근무조
    /// </summary>
    public string? Shift { get; set; }
    /// <summary>
    /// 작업자 사원번호.
    /// </summary>
    /// <remarks>employee 테이블의 year + gender + sequence</remarks>
    public string? EmployeeNumber { get; set; }
    /// <summary>
    /// 누적 생산량
    /// </summary>
    public int Total { get; set; }
}
