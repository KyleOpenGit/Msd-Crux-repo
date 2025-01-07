
namespace MSD.Crux.API.Models.DTO;

/// <summary>
/// 이미 등록된 User의 login_id, login_pw  등록요청 DTO 클래스
/// </summary>
/// <remarks>사원번호는 PUT URL 파라미터에서 처리</remarks>
public class UserLoginRegiReqDto
{
    public string LoginId { get; set; } = string.Empty;
    public string LoginPw { get; set; } = string.Empty;
}
