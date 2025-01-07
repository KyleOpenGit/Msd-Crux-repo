
using MSD.Crux.API.Models.DTO;

namespace MSD.Crux.API.Services;

/// <summary>
/// 유저 로그인 ID/PW 생성과  로그인시 JWT 발급 관련 서비스 인터페이스
/// </summary>
public interface IUserLoginService
{
    /// <summary>
    /// 로그인 ID와 비밀번호를 등록
    /// </summary>
    /// <param name="employeeNumber">사원번호</param>
    /// <param name="loginId">로그인 ID</param>
    /// <param name="password">비밀번호</param>
    /// <returns>등록된 유저 정보</returns>
    Task<UserLoginIdRegiRspDto> RegisterLoginIdAsync(int employeeNumber, string loginId, string password);
}
