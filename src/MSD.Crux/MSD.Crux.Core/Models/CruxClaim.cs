using System.Security.Claims;

namespace MSD.Crux.Core.Models;

/// <summary>
/// Crux 서버에서 사용되는 클레임 목록을 담은 객체.
/// JWT 클레임 정보를 캡슐화한 모델 클래스.
/// Jwt에 포함할 클레임 목록을 한눈에 알아보기 쉽고 중앙에서 관리 하기 위한 클래스.
/// </summary>
public class CruxClaim
{
    /// <summary>
    /// 로그인 ID
    /// </summary>
    public string LoginId { get; set; } = string.Empty;
    /// <summary>
    /// 직원 이름
    /// </summary>
    public string EmployeeName { get; set; } = string.Empty;
    /// <summary>
    /// 사원 번호
    /// </summary>
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 역할(Role) 목록 (쉼표 구분)
    /// </summary>
    public string Roles { get; set; } = string.Empty;

    /// <summary>
    /// CruxClaim 멤버를 사용해 필요한 JWT 클레임 목록을 생성한다.
    /// </summary>
    /// <returns>JWT 클레임 리스트</returns>
    public IEnumerable<Claim> ToClaims()
    {
        return new List<Claim>
               {
                   new Claim(ClaimTypes.NameIdentifier, LoginId),
                   new Claim(ClaimTypes.Name, EmployeeName),
                   new Claim("EmployeeNumber", EmployeeNumber.ToString()),
                   new Claim(ClaimTypes.Role, Roles)
               };
    }

    /// <summary>
    /// JWT 클레임 목록으로 <see cref="CruxClaim"/>를 생성한다.
    /// </summary>
    /// <param name="claims">JWT 클레임 리스트</param>
    /// <returns><see cref="CruxClaim"/> 객체</returns>
    public static CruxClaim FromClaims(IEnumerable<Claim> claims)
    {
        return new CruxClaim
        {
            LoginId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            EmployeeName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty,
            EmployeeNumber = int.TryParse(claims.FirstOrDefault(c => c.Type == "EmployeeNumber")?.Value, out int empNum) ? empNum : 0,
            Roles = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty
        };
    }
}
