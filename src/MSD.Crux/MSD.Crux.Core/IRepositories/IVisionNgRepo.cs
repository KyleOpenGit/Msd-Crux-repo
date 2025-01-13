using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IRepositories;

public interface IVisionNgRepo
{
    Task AddAsync(VisionNg visionNg);
}
