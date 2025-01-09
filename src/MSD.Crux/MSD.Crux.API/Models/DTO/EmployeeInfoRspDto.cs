namespace MSD.Crux.API.Models;

/// <summary>
/// 편의성을 높인 직원정보 객체 DTO.
/// DB의 year,gender,squence 복합키를 단일 직원번호로 제공.
/// </summary>
public class EmployeeInfoRspDto
{
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 남 || 여
    /// </summary>
    public string? Sex { get; set; }
    /// <summary>
    /// 직원 이름
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// 소속 부서
    /// </summary>
    public string? Department { get; set; }
    /// <summary>
    /// 근무 교대조 (예: A, B)
    /// </summary>
    public string? Shift { get; set; }
    /// <summary>
    /// 직책 (예: 팀장, 사원)
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// 입사일
    /// </summary>
    public DateTime? JoinDate { get; set; }
}
