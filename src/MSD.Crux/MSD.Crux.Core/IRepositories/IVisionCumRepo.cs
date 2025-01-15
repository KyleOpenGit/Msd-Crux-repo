
using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

/// <summary>
/// vision_cum 테이블 레포지토리 인터페이스
/// </summary>
public interface IVisionCumRepo
{
    /// <summary>
    /// 새로운 누적검사량 레코드 추가
    /// </summary>
    /// <param name="visionCum">품질 검사 누적량 정보를 가진 VisionCum 객체</param>
    /// <returns>비동기 작업 완료</returns>
    Task<int> AddVisionCumAsync(VisionCum visionCum);
}

