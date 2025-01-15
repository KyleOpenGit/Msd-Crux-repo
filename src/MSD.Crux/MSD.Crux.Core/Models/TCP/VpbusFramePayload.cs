namespace MSD.Crux.Core.Models.TCP;

public class VpbusFramePayload
{
    public string LineId { get; set; } = string.Empty;
    public long Time { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty;
    public int EmployeeNumber { get; set; }
    public int Total { get; set; }
}
