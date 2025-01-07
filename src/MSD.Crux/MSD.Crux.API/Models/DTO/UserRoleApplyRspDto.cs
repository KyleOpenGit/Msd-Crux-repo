namespace MSD.Crux.API.Models;

/// <summary>
/// User 등록 및 권한 신청 목록 조회 Dto.
/// User 레코드로 추가해야할 직원과 권한
/// </summary>
public class UserRoleApplyRspDto
{
    /// <summary>
    /// 직워번호
    /// </summary>
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 신청권한
    /// </summary>
    public string? ApplyUserRoles { get; set; }
}
