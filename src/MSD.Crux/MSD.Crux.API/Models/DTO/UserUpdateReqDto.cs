// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace MSD.Crux.API.Models;

/// <summary>
/// 유저 정보 업데이트 요청 DTO
/// </summary>
public class UserUpdateReqDto
{
    /// <summary>
    /// 정보를 변경할 유저의 직원번호
    /// </summary>
    public string EmployeeNumber { get; set; } = string.Empty;
    /// <summary>
    /// 변경될 직원이름
    /// </summary>
    public string? Name { get; set; } = string.Empty;
    /// <summary>
    /// 변경될 유저권한 목록
    /// </summary>
    public string? Roles { get; set; }
    /// <summary>
    /// 로그인 ID/PW/salt 초기화 여부. use 테이블값을 비우서 재등록 가능하도록 할지 여부.
    /// </summary>
    public bool ResetLoginCredentials { get; set; }
}
