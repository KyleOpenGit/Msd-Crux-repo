// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace MSD.Crux.API.Models.DTO;

public class UserLoginRspDto
{
    /// <summary>
    /// User 레코드 Id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 시스템 로그인 아이디
    /// </summary>
    public string? LoginId { get; set; }
    /// <summary>
    /// 사원번호 (employee 테이블의 year + gender + sequence)
    /// </summary>
    public int EmployeeNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Base64 직원 사진 데이터
    /// </summary>
    public string Photo { get; set; } = string.Empty;
    /// <summary>
    /// 근무 교대조 ( A, B)
    /// </summary>
    public string Shift { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public string JwtPublicKey { get; set; } = string.Empty;
}
