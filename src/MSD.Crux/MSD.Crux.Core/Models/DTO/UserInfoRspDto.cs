
namespace MSD.Crux.Core.Models;

public class UserInfoRspDto
{
    /// <summary>
    /// user_id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 사원번호 (employee 테이블의 year + gender + sequence)
    /// </summary>
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 로그인 아이디 (NULL 가능)
    /// </summary>
    public string? LoginId { get; set; }
    /// <summary>
    /// 직원 이름 (100글자 이내)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 유저 권한 (복수 조합 가능, 쉼표로 구분된 문자열 255글자 이내)
    /// </summary>
    public string? Roles { get; set; }
    /// <summary>
    /// 근무 교대조 (예: A, B)
    /// </summary>
    public string? Shift { get; set; }
}
