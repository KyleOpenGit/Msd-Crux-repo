using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.Repositories;

/// <summary>
/// User 테이블에 대한 레포지토리 인터페이스
/// </summary>
public interface IUserRepo
{
    /// <summary>
    /// 유저 ID(PK)로 특정 유저 조회
    /// </summary>
    /// <param name="id">유저 ID(PK)</param>
    /// <returns>조회된 유저 정보 또는 null</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Login ID로 특정 유저 조회
    /// </summary>
    /// <param name="loginId">유저 로그인 ID</param>
    /// <returns>조회된 유저 정보 또는 null</returns>
    Task<User?> GetByLoginIdAsync(string loginId);

    /// <summary>
    /// Employee Number로 특정 유저 조회
    /// </summary>
    /// <param name="employeeNumber">사원번호</param>
    /// <returns>조회된 유저 정보 또는 null</returns>
    Task<User?> GetByEmployeeNumberAsync(int employeeNumber);

    /// <summary>
    /// 모든 유저 조회
    /// </summary>
    /// <returns>유저 목록</returns>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// 새로운 유저 추가
    /// </summary>
    /// <param name="user">추가할 유저 정보를 가진 User 객체</param>
    /// <returns>비동기 작업 완료</returns>
    Task AddAsync(User user);

    /// <summary>
    /// 기존 유저 정보 업데이트
    /// </summary>
    /// <param name="user">업데이트할 유저 정보를 가진 User 객체</param>
    /// <returns>비동기 작업 완료</returns>
    Task UpdateAsync(User user);

    /// <summary>
    /// 유저 삭제
    /// </summary>
    /// <param name="id">삭제할 유저의 ID</param>
    /// <returns>비동기 작업 완료</returns>
    Task DeleteAsync(int id);
}
