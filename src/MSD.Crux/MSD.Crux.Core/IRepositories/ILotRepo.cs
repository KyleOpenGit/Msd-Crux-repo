using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories
{
    /// <summary>
    /// Lot 테이블에 대한 레포지토리 인터페이스
    /// </summary>
    public interface ILotRepo
    {
        /// <summary>
        /// 생산이 끝난 Lot를 조회
        /// </summary>
        /// <returns>생산 완료된 Lot 리스트 </returns>
        Task<List<Lot?>> GetAllCompletedLotsAsync();

        /// <summary>
        /// Lot ID를 기반으로 Lot를 조회
        /// </summary>
        /// <param name="id">조회할 Lot ID</param>
        /// <returns>조회된 Lot 정보 또는 null</returns>
        Task<Lot?> GetByIdAsync(string id);

        /// <summary>
        /// 특정 제품 ID와 날짜에 대한 최신 순번 조회 (새로운 Lot Id 발급용)
        /// </summary>
        /// <remarks>
        /// - 주어진 `partId`와 `date`로 구성된 접두사(Prefix)를 기준으로,
        ///   테이블에서 해당 Prefix로 시작하는 Lot ID를 검색합니다.
        /// - Lot ID 형식은 `"{partId}-{date:yyyyMMdd}-{sequence}"`와 같습니다.
        /// - 예: "AAAA1-20250101-1", "AAAA1-20250101-2", ...
        /// </remarks>
        /// <param name="partId">Lot ID에 포함될 제품 ID (예: "AAAA1")</param>
        /// <param name="date">Lot ID에 포함될 날짜 (yyyy-MM-dd 형식)</param>
        /// <returns>
        /// - 같은 Prefix로 시작하는 Lot ID 중 가장 큰 순번(`sequence`) 반환.
        /// - 해당 Prefix로 시작하는 Lot ID가 없으면 기본값 `0` 반환.
        /// </returns>
        Task<int> GetLatestSequenceOfIdAsync(string partId, DateTime date);

        /// <summary>
        /// 새로운 Lot를 추가
        /// </summary>
        /// <param name="lot">추가할 Lot 엔티티</param>
        /// <returns>비동기 작업</returns>
        Task AddAsync(Lot lot);

        /// <summary>
        /// 최소 Lot 정보를 추가 (lot ID 발급 전용).
        /// </summary>
        /// <param name="lot">Lot 엔티티 객체</param>
        /// <remarks> Id, PartId, IssuedTime 세가지 정보에대한 칼럼만 채운다 </remarks>
        Task AddMinimalAsync(Lot lot);

        /// <summary>
        /// 기존 Lot 데이터를 업데이트
        /// </summary>
        /// <param name="lot">업데이트할 Lot 엔티티</param>
        /// <returns>비동기 작업</returns>
        Task UpdateAsync(Lot lot);

        /// <summary>
        /// Lot ID를 기반으로 Lot 데이터를 삭제
        /// </summary>
        /// <param name="id">삭제할 Lot ID</param>
        /// <returns>비동기 작업</returns>
        Task DeleteAsync(string id);
    }
}
