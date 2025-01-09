namespace MSD.Crux.API.Models;

/// <summary>
/// User 엔티티 클래스 - DB의 user 테이블 매핑.
/// HMI, MES, 시스템 등록 유저
/// </summary>
/// <remarks>LoginId, LoginPw , Salt 는 모두 NULL 이거나 모두 NOT NULL 이어야한다. </remarks>
public class User
{
    /// <summary>
    /// PK, Auto Increment
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
    /// 로그인 비밀번호 (NULL 가능, PBKDF2로 해싱된 값)
    /// </summary>
    public string? LoginPw { get; set; }
    /// <summary>
    /// 비밀번호 해싱에 사용된 Salt (22글자, NULL 가능)
    /// </summary>
    public string? Salt { get; set; }
    /// <summary>
    /// 직원 이름 (100글자 이내)
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 유저 권한 (복수 조합 가능, 쉼표로 구분된 문자열 255글자 이내)
    /// </summary>
    public string? Roles { get; set; }
}
