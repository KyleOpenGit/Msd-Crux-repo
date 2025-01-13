namespace MSD.Crux.Core.Models
{
    /// <summary>
    /// 엔티티 클래스 - DB의 Lot 테이블 매핑.
    /// </summary>
    public class Lot
    {
        /// <summary>
        /// 생산 로트번호
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 생산 품목 번호
        /// </summary>
        public string PartId { get; set; } = string.Empty;
        /// <summary>
        /// 생산 라인코드
        /// </summary>
        public string LineId { get; set; } = string.Empty;
        /// <summary>
        /// 로트 번호의 발행시간
        /// </summary>
        public DateTime IssuedTime { get; set; }
        /// <summary>
        /// 생산수량
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 양품수량
        /// </summary>
        public int CompletedQty { get; set; }
        /// <summary>
        /// 비전 라인코드
        /// </summary>
        public string? VisionLineIds { get; set; }
        /// <summary>
        /// 생산 시작 시간
        /// </summary>
        public DateTime InjectionStart { get; set; }
        /// <summary>
        /// 생산 완료 시간
        /// </summary>
        public DateTime InjectionEnd { get; set; }
        /// <summary>
        /// 생산 작업자 사원번호
        /// </summary>
        public string? InjectionWorker { get; set; }
        /// <summary>
        /// 비전 검사 시작시간
        /// </summary>
        public DateTime? VisionStart { get; set; }
        /// <summary>
        /// 비전 검사 완료시간
        /// </summary>
        public DateTime? VisionEnd { get; set; }
        /// <summary>
        /// 품질 작업자 사원번호
        /// </summary>
        public string? VisionWorker { get; set; }
        /// <summary>
        /// 자재 공급사
        /// </summary>
        public string? Supplier { get; set; }
        /// <summary>
        /// 메모
        /// </summary>
        public string? Note { get; set; }
    }
}
