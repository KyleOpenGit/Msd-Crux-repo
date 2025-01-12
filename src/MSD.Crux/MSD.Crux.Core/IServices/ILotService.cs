using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices
{
    public interface ILotService
    {
        /// <summary>
        /// 생산이 끝난 Lot를 조회
        /// </summary>
        /// <returns>조회된 로트정보 또는 null</returns>
        Task<List<Lot?>> GetAllCompletedLotsAsync();
    }
}
