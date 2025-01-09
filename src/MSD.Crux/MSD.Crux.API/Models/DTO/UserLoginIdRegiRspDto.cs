
namespace MSD.Crux.API.Models.DTO;

/// <summary>
///
/// user 로그인ID/PW 등록에대한 응답 DTO
/// </summary>
public class UserLoginIdRegiRspDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EmployeeNumber { get; set; }

}
