namespace MSD.Crux.API.Models;

/// <summary>
/// Employee 엔티티 클래스 - DB의 employee 테이블 매핑.
/// </summary>
public class Employee
{
    /// <summary>
    /// 입사 연도
    /// </summary>
    public short Year { get; set; }
    /// <summary>
    /// 밀레니엄 성별 (1, 2, 3, 4)
    /// </summary>
    public MillenniumGender Gender { get; set; }
    /// <summary>
    /// 해당 연도 입사 순서
    /// </summary>
    public short Sequence { get; set; }
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
    /// <summary>
    /// 요청된 유저 권한 (쉼표 구분된 문자열)
    /// </summary>
    public string? RequestingUserRoles { get; set; }
    /// <summary>
    /// 직원 사진 (Base64 인코딩된 문자열)
    /// </summary>
    public string? Photo { get; set; }
}

/// <summary>
/// Millennium 기준 주민등록 성별 번호
/// </summary>
public enum MillenniumGender
{
    /// <summary>
    /// 2000년 이전 남성 (1)
    /// </summary>
    Male = 1,
    /// <summary>
    /// 2000년 이전 여성 (2)
    /// </summary>
    Female = 2,
    /// <summary>
    /// // 2000년 이후 남성 (3)
    /// </summary>
    MMale = 3,
    /// <summary>
    /// 2000년 이후 여성 (4)
    /// </summary>
    MFemale = 4
}
