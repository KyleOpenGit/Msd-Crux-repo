using System.ComponentModel.DataAnnotations;

namespace MSD.Crux.API.Models.DTO;

/// <summary>
/// 로그인 요청 DTO
/// </summary>
public class UserLoginReqDto
{
    [Required]
    public string? LoginId { get; set; }
    [Required]
    public string? LoginPw { get; set; }
}
