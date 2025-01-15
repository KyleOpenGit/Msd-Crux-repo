namespace MSD.Crux.Core.Models.TCP;

/// <summary>
/// TCP 소켓통신 커스텀 프로토콜 (VPBUS)의 헤더를 나타내는 모델 클래스.
/// 참고: 팀프로젝트의 소켓통니 프로토콜 정의 문서
/// </summary>
public class VpbusFrameHeader
{
    /// <summary>
    /// 헤더인덱스[0]번째 바이트. Frame 형태 구분.
    /// </summary>
    /// <code>
    /// 0 : JWT
    /// 1: 사출생산 누적(injection_cum)
    /// 2: 비전검사 누적 vision_cum
    /// </code>
    public FrameType FrameType { get; set; }
    /// <summary>
    /// 헤더인덱스[1]과 [2]번째 바이트 = 2 bytes를 수용하는 타입.
    /// Payload의 바이트 길이
    /// </summary>
    public ushort MessageLength { get; set; }
    /// <summary>
    /// 헤더인덱스[3]번째 바이트. 프로토콜 버전. 바이트 커스텀 Frame 데이터 포맷 버전관리용
    /// </summary>
    /// <code>
    /// 1 : version 1.0
    /// </code>
    public byte MessageVersion { get; set; } = 1;
    /// <summary>
    /// 헤더인덱스[4]번째 바이트. 클라이언트 역할 번호
    /// </summary>
    /// <code>
    /// 0:생산 HMI,  1:품질 HMI,  2:스카다(WPF)
    /// </code>
    public byte Role { get; set; }
    /// <summary>
    /// 헤더인덱스[5]번째 바이트. 스페어 바이트
    /// </summary>
    public byte Reserved { get; set; } = 0;
}

public enum FrameType
{
    /// <summary>
    /// JWT 인증용 Frame 프로토콜
    /// </summary>
    Jwt = 0,
    /// <summary>
    /// 누적생상량 Frame 프로토콜
    /// </summary>
    Injection = 1,
    /// <summary>
    /// 누적검사량 Frame 프로토콜
    /// </summary>
    Vision = 2
}
