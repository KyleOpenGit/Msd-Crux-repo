
using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

/// <summary>
/// injection_cum 테이블 레포지토리 인터페이스
/// </summary>
public interface IInjectionCumRepo
{
    /// <summary>
    /// 새로운 누적생산량 레코드 추가
    /// </summary>
    /// <param name="injectionCum">누적 생산량 정보를 가진 InjectionCum 객체</param>
    /// <returns>비동기 작업 완료</returns>
    Task<int> AddInjectionCumAsync(InjectionCum injectionCum);
}
