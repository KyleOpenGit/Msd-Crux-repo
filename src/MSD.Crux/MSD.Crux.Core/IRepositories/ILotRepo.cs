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
        /// 새로운 Lot를 추가
        /// </summary>
        /// <param name="lot">추가할 Lot 엔티티</param>
        /// <returns>비동기 작업</returns>
        Task AddAsync(Lot lot);

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
