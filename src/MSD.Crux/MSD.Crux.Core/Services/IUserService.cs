using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.Services;

/// <summary>
/// user 등록과 정보수정, 검색 로직에대한 계약
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 새로운 User 등록.
    /// </summary>
    /// <param name="request">등록할 직원의 직원번호, 이름, 권한</param>
    /// <returns>등록성공한 user 정보</returns>
    Task<UserRegiRspDto> RegisterUserAsync(UserRegiReqDto request);

    /// <summary>
    /// 레코드 id로 기존 유저를 찾아서 정보 수정
    /// </summary>
    /// <param name="id">user 레코드 id</param>
    /// <param name="updateReqDto">수정할 이름과 권한 + 로그인ID/PW/Salt 초기화 여부</param>
    /// <returns></returns>
    Task UpdateUserByIdAsync(int id, UserUpdateReqDto updateReqDto);

    /// <summary>
    /// 직원번호로 기존 유저를 찾아서 정보 수정
    /// </summary>
    /// <param name="employeeNumber">정보를 수정할 직원의 직원번호</param>
    /// <param name="updateReqDto">수정할 이름과 권한 + 로그인ID/PW/Salt 초기화 여부</param>
    /// <returns></returns>
    Task UpdateUserByEmployeeNumberAsync(int employeeNumber, UserUpdateReqDto updateReqDto);

    /// <summary>
    /// 모든 유저 목록 찾기
    /// </summary>
    /// <returns>공개 가능한 유저 정보가 담긴 목록</returns>
    Task<IEnumerable<UserInfoRspDto>> GetAllUsersAsync();

    /// <summary>
    /// 레코드 id로 유저 찾기
    /// </summary>
    /// <param name="id">user 테이블 레코드 id</param>
    /// <returns>공개 가능한 유저 정보가 담긴 목록</returns>
    Task<UserInfoRspDto?> GetUserByIdAsync(int id);

    /// <summary>
    /// 직원번호로 유저 찾기
    /// </summary>
    /// <param name="employeeNumber">직원번호</param>
    /// <returns>공개 가능한 유저 정보가 담긴 목록</returns>
    Task<UserInfoRspDto?> GetUserByEmployeeNumberAsync(int employeeNumber);

    /// <summary>
    /// 특정 user 삭제
    /// </summary>
    /// <param name="id"> user 테이블 레코드id</param>
    /// <returns></returns>
    Task DeleteUserAsync(int id);

    /// <summary>
    /// User 등록을 신청한 모든 employee 목록을 조회.
    /// </summary>
    /// <returns>User 등록을 신청한 employee 정보: 직원번호,이름 + roles </returns>
    Task<IEnumerable<UserRoleApplyRspDto>> GetUserRoleApplicationsAsync();
}
