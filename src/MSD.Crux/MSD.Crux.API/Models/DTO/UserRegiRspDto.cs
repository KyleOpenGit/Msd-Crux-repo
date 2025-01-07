// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace MSD.Crux.API.Models;

/// <summary>
/// 유저 등록에대한 응답 DTO. 부여된 레코드 id를 포함한다.
/// </summary>
public class UserRegiRspDto
{
    /// <summary>
    /// user_id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 직원번호. employee테이블의 year + gender + sequence
    /// </summary>
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 직원이름
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// 유저 권한(복수가능 문자열. 쉼표구분)
    /// </summary>
    public string? Roles { get; set; }
}
