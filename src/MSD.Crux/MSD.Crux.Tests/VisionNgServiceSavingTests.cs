using Microsoft.Extensions.Configuration;
using Moq;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;
using MSD.Crux.Infra.Services;

namespace MSD.Crux.Tests.Services;

/// <summary>
/// VisionNgService의 SaveVisionNgAsync 메서드 테스트
/// 모킹을 통해 LotRepo의 동작을 시뮬레이션하면 유닛 테스트에서 독립적으로 서비스 계층을 테스트 한다.
/// </summary>
[TestFixture]
public class VisionNgServiceSavingTests
{
    private Mock<IVisionNgRepo> _mockRepo;
    private Mock<ILotRepo> _mockLotRepo;
    private VisionNgService _service;
    private string _testBasePath;
    private const string Base64ImageData =
        "iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAAADTED8xAAADMElEQVR4nOzVwQnAIBQFQYXff81RUkQCOyDj1YOPnbXWPmeTRef+/3O/OyBjzh3CD95BfqICMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMO0TAAD//2Anhf4QtqobAAAAAElFTkSuQmCC\n";
    /// <summary>
    /// 테스트 후 디렉토리를 삭제할지 여부를 제어
    /// </summary>
    private bool _deleteAfterTest = false; // 기본값: 삭제하지 않음

    /// <summary>
    /// 테스트 환경을 설정
    /// - Mock 레포지토리 초기화
    /// - 임시 디렉토리 생성
    /// - 서비스 초기화
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // 임시 디렉토리 생성. Temp 폴더로 경로 하위에 생성 - macOS: "/var/folders/91/0gjjn3r16pj0079jkfwd373m0000gp/T/" 같은 하위 경로
        _testBasePath = Path.Combine(Path.GetTempPath(), "VisionNgTest");
        if (Directory.Exists(_testBasePath))
        {
            Directory.Delete(_testBasePath, true);
        }

        //  레포지토리 Mock 객체 생성
        _mockRepo = new Mock<IVisionNgRepo>();
        _mockLotRepo = new Mock<ILotRepo>();

        // IConfiguration 설정
        Dictionary<string, string>? configData = new() { { "ImageStorage:BasePath", _testBasePath } };
        IConfigurationRoot? configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

        // 서비스 초기화
        _service = new VisionNgService(_mockRepo.Object, _mockLotRepo.Object, configuration);
    }

    /// <summary>
    /// SaveVisionNgAsync 메서드가 올바르게 이미지를 저장하고, DB에 데이터를 기록하는지 테스트.
    /// </summary>
    /// <remarks>
    /// - 이미지가 지정된 경로에 저장되는지 확인
    /// - DB 저장 메서드가 호출되는지 확인
    /// </remarks>
    [Test]
    public async Task SaveVisionNgAsync_ShouldSaveImageToCorrectPath_AndRecordToDb()
    {
        // Arrange
        byte[] mockImageData = Convert.FromBase64String(Base64ImageData); // Base64 → 바이트 배열
        VisionNgReqDto? requestDto = new() { LotId = "LOT123", LineId = "LINE01", DateTime = DateTime.UtcNow, NgLabel = NgType.Crack, Img = mockImageData };

        // Mock LotRepo 동작 설정
        _mockLotRepo.Setup(repo => repo.GetByIdAsync("LOT123")).ReturnsAsync(new Lot { Id = "LOT123", PartId = "PART001" });

        // Act
        await _service.SaveVisionNgAsync(requestDto);

        // Assert: 디렉토리와 파일 확인
        string expectedDirectory = Path.Combine(_testBasePath, DateTime.UtcNow.ToString("yyyy/MM/dd"));
        Console.WriteLine($"이미지 저장 경로: {expectedDirectory}"); // 저장 경로 출력

        Assert.That(Directory.Exists(expectedDirectory), Is.True, "저장 디렉토리가 생성되지 않았습니다.");

        string[] files = Directory.GetFiles(expectedDirectory);
        Assert.That(files.Length, Is.EqualTo(1), "이미지가 저장되지 않았습니다.");
        string savedFilePath = files[0];
        Console.WriteLine($"저장된 파일 경로: {savedFilePath}"); // 파일 경로 출력
        Assert.That(File.Exists(savedFilePath), Is.True, "저장된 파일이 존재하지 않습니다.");

        // DB 저장 호출 검증
        _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<VisionNg>()), Times.Once, "DB 저장이 호출되지 않았습니다.");
    }

    /// <summary>
    /// 테스트 종료 후 설정된 디렉토리를 삭제할지 선택에 따라 삭제
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // 테스트 종료 후 디렉토리 삭제 여부를 플래그로 제어
        if (_deleteAfterTest && Directory.Exists(_testBasePath))
        {
            Directory.Delete(_testBasePath, true);
        }
    }
}
