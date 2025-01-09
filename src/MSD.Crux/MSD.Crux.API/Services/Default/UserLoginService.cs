using MSD.Crux.API.Helpers;
using MSD.Crux.API.Models;
using MSD.Crux.API.Models.DTO;
using MSD.Crux.API.Repositories;

namespace MSD.Crux.API.Services;

public class UserLoginService(IUserRepo _userRepo, IEmployeeRepo _employeeRepo, IConfiguration _configuration) : IUserLoginService
{
    /// <summary>
    /// 이미 등록된 User의 로그인 ID와 비밀번호를 등록.
    /// id,pw,salt 칼럼을 검사해서 이미 로그인ID/PW가 등록되어있는지 확인해서 등록이 안되어 있을 때만 등록한다.
    /// </summary>
    /// <param name="employeeNumber">사원번호</param>
    /// <param name="loginId">등록할 로그인 ID</param>
    /// <param name="password">등록할 비밀번호(raw)</param>
    /// <returns>등록된 유저 정보</returns>
    /// <exception cref="InvalidOperationException">BadRequest로 catch될 예외 객체</exception>
    public async Task<UserLoginIdRegiRspDto> RegisterLoginIdAsync(int employeeNumber, string loginId, string password)
    {
        // 사원번호로 유저 검색
        User? user = await _userRepo.GetByEmployeeNumberAsync(employeeNumber);
        if (user == null)
        {
            throw new InvalidOperationException($"사원번호 {employeeNumber}의 User를 찾을 수 없습니다. 전산팀에 문의 하십시오.");
        }

        if (HasLoginCredentials(user))
        {
            throw new InvalidOperationException($"{user.Name}({user.EmployeeNumber})의 Login ID 와 password는 이미 존재합니다. 비밀번호 초기화를 원하시면 생산관리팀을 통해서 전산팀에 서면 요청하십시오");
        }

        User? existingUserWithLoginId = await _userRepo.GetByLoginIdAsync(loginId);
        if (existingUserWithLoginId != null)
        {
            throw new InvalidOperationException("이미 존재하는 Login ID입니다. 다른 ID를 입력해주세요");
        }

        string hashedPassword = PwdHasher.GenerateHash(password, out string salt);

        // 5. 유저 정보 업데이트
        user.LoginId = loginId;
        user.LoginPw = hashedPassword;
        user.Salt = salt;
        await _userRepo.UpdateAsync(user);

        return new UserLoginIdRegiRspDto { Id = user.Id, Name = user.Name, EmployeeNumber = user.EmployeeNumber };
    }

    /// <summary>
    /// 로그인 정보를 검증 한 후 JWT 를 반환한다.
    /// </summary>
    /// <param name="loginId">유저 로그인 ID </param>
    /// <param name="password">유저 로그인 PW </param>
    /// <returns>로그인 성공한 유저 정보와 Jwt</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<UserLoginRspDto> LoginAsync(string loginId, string password)
    {
        // User 테이블에서 로그인 ID로 사용자 조회
        var user = await _userRepo.GetByLoginIdAsync(loginId);
        if (user == null)
        {
            throw new InvalidOperationException("로그인 ID가 유효하지 않습니다.");
        }

        // 비밀번호 검증
        if (!PwdHasher.VerifyHash(password, user.Salt, user.LoginPw))
        {
            throw new UnauthorizedAccessException("비밀번호가 유효하지 않습니다.");
        }

        var employee = await _employeeRepo.GetByEmployeeNumberAsync(user.EmployeeNumber);
        string token = JwtHelper.GenerateToken(user, _configuration);

        // 응답 DTO 생성
        return new UserLoginRspDto
        {
            Id = user.Id,
            LoginId = user.LoginId,
            EmployeeNumber = user.EmployeeNumber,
            Name = user.Name,
            Photo = employee?.Photo ?? string.Empty,
            Shift = employee?.Shift ?? string.Empty,
            JwtToken = token,
            JwtPublicKey = _configuration["Jwt:PublicKey"]
        };
    }

    /// <summary>
    ///  User 객체에 이미 로그인ID정보가 등록되어있는지 확인
    /// </summary>
    /// <param name="user">테이블에 등록된 user 객체</param>
    /// <returns>ID, PW, Salt가 null이거나 비어있는지 여부 </returns>
    private bool HasLoginCredentials(User user)
    {
        return !string.IsNullOrEmpty(user.LoginId) || !string.IsNullOrEmpty(user.LoginPw) || !string.IsNullOrEmpty(user.Salt);
    }
}
