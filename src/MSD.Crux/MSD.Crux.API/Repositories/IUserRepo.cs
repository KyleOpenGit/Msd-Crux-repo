using MSD.Crux.API.Models;

namespace MSD.Crux.API.Repositories;

/// <summary>
/// User 테이블에 대한 레포지토리 인터페이스
/// </summary>
public interface IUserRepo
{
    /// <summary>
    /// 유저 ID(PK)로 특정 유저 조회
    /// </summary>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Login ID로 특정 유저 조회
    /// </summary>
    Task<User?> GetByLoginIdAsync(string loginId);

    /// <summary>
    /// Employee Number로 특정 유저 조회
    /// </summary>
    Task<User?> GetByEmployeeNumberAsync(int employeeNumber);

    /// <summary>
    /// 모든 유저 조회
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// 새로운 유저 추가
    /// </summary>
    Task AddAsync(User user);

    /// <summary>
    /// 기존 유저 정보 업데이트
    /// </summary>
    Task UpdateAsync(User user);

    /// <summary>
    /// 유저 삭제
    /// </summary>
    Task DeleteAsync(int id);
}
