namespace MSD.Crux.API.Models;

/// <summary>
/// User 엔티티 클래스 - DB의 user 테이블 매핑.
/// HMI, MES, 시스템 등록 유저
/// </summary>
public class User
{
    /// <summary>
    /// PK, Auto Increment
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 로그인 아이디
    /// </summary>
    public string LoginId { get; set; } = string.Empty;
    /// <summary>
    ///  로그인 비번: password + salt가 단방향 암호화 해싱된 문자열
    /// </summary>
    public string LoginPw { get; set; } = string.Empty;
    /// <summary>
    ///  해싱되기 전에 password 뒤에 추가된 22글자고정 랜덤 문자열
    /// </summary>
    public string Salt { get; set; } = string.Empty;
    /// <summary>
    /// 직원 이름
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 사원번호
    /// </summary>
    public string? EmployeeNumber { get; set; }
    /// <summary>
    /// 유저 권한 (복수 조합 가능. 쉼표로 구분)
    /// </summary>
    public string? Roles { get; set; } = "work";
    /// <summary>
    /// 프사
    /// </summary>
    public string? ProfileImg { get; set; }
    /// <summary>
    /// 프사소개
    /// </summary>
    public string? ProfileText { get; set; }
}
