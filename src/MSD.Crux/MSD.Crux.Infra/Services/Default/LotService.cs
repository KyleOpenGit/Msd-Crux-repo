using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Core.Models;

namespace MSD.Crux.Infra.Services.Default
{
    public class LotService(ILotRepo _lotRepo) : ILotService
    {
        public async Task<List<Lot?>> GetAllCompletedLotsAsync()
        {
            return await _lotRepo.GetAllCompletedLotsAsync();
        }
    }
}
