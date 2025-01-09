namespace MSD.Crux.Core.Models;

/// <summary>
/// 새로운 유저 등록 및 권한 승인 요청 DTO
/// </summary>
public class UserRegiReqDto
{
    /// <summary>
    /// 직원번호 (employee 테이블의 year + gender + sequence)
    /// </summary>
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 직원이름 (100글자 이내)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 유저 권한 (복수 조합 가능, 쉼표로 구분된 문자열 255글자 이내)
    /// </summary>
    public string? Roles { get; set; }
}
