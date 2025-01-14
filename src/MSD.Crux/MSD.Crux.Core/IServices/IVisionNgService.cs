using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.IServices;

public interface IVisionNgService
{
    Task SaveVisionNgAsync(VisionNgReqDto request);
}
