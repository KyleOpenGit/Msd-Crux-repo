namespace MSD.Crux.Core.Models.TCP;

/// <summary>
/// TCP 소켓통신 커스텀 프로토콜 (VPBUS)의 페이로드를 나타내는 모델 클래스.
/// 참고: 팀프로젝트의 소켓통신 프로토콜 정의 문서
/// </summary>
public class VpbusFramePayload
{
    /// <summary>
    /// 페이로드 첫 4바이트. 전체프레임 인덱스: 6-9
    /// </summary>
    public string LineId { get; set; } = string.Empty;
    /// <summary>
    /// 페이로드 두번째 8바이트. 전체프레임 인덱스: 10-17
    /// </summary>
    public long Time { get; set; }
    /// <summary>
    /// 페이로드 세번째 20바이트. 전체프레임 인덱스: 18-37
    /// </summary>
    public string LotId { get; set; } = string.Empty;
    /// <summary>
    /// 페이로드 네번째 4바이트. 전체프레임 인덱스: 38-41
    /// </summary>
    public string Shift { get; set; } = string.Empty;
    /// <summary>
    /// 페이로드 다섯번째 4바이트. 전체프레임 인덱스: 42-45
    /// </summary>
    public int EmployeeNumber { get; set; }
    /// <summary>
    /// 페이로드 여섯번째 4바이트. 전체프레임 인덱스: 46-49
    /// </summary>
    public int Total { get; set; }
}
