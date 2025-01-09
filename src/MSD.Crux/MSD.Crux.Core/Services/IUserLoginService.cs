using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.Services;

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

    /// <summary>
    /// 로그인 ID와 비밀번호를 통해 사용자를 인증하고 JWT 발급
    /// </summary>
    /// <param name="loginId">로그인 ID</param>
    /// <param name="password">비밀번호</param>
    /// <param name="_configuration">IConfiguration</param>
    /// <returns>인증된 사용자 정보와 JWT 토큰</returns>
    Task<UserLoginRspDto> LoginAsync(string loginId, string password);
}
