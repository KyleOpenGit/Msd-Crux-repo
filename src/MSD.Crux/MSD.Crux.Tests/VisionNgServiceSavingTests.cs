using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.Models;
using MSD.Crux.Infra.Services;
using NUnit.Framework;

namespace MSD.Crux.Tests.Services
{
    [TestFixture]
    public class VisionNgServiceSavingTests
    {
        private Mock<IVisionNgRepo> _mockRepo;
        private VisionNgService _service;
        private string _testBasePath;

        private const string Base64ImageData = "iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAAADTED8xAAADMElEQVR4nOzVwQnAIBQFQYXff81RUkQCOyDj1YOPnbXWPmeTRef+/3O/OyBjzh3CD95BfqICMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMK0CMO0TAAD//2Anhf4QtqobAAAAAElFTkSuQmCC\n";

        /// <summary>
        /// 테스트 후 디렉토리를 삭제할지 여부를 제어
        /// </summary>
        private bool _deleteAfterTest = false; // 기본값: 삭제하지 않음

        [SetUp]
        public void SetUp()
        {
            // 임시 디렉토리 생성
            _testBasePath = Path.Combine(Path.GetTempPath(), "VisionNgTest");
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }

            // Mock 레포지토리 생성
            _mockRepo = new Mock<IVisionNgRepo>();

            // IConfiguration 설정
            Dictionary<string, string>? configData = new () { { "ImageStorage:BasePath", _testBasePath } };
            IConfigurationRoot? configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

            // 서비스 초기화
            _service = new VisionNgService(_mockRepo.Object, configuration);
        }

        [Test]
        public async Task SaveVisionNgAsync_ShouldSaveImageToCorrectPath_AndRecordToDb()
        {
            // Arrange
            byte[] mockImageData = Convert.FromBase64String(Base64ImageData); // Base64 → 바이트 배열
            VisionNgReqDto? requestDto = new () { LotId = "LOT123", LineId = "LINE01", DateTime = DateTime.UtcNow, NgLabel = NgType.Crack, Img = mockImageData };

            // Act
            await _service.SaveVisionNgAsync(requestDto);

            // Assert: 디렉토리와 파일 확인
            string expectedDirectory = Path.Combine(_testBasePath, DateTime.UtcNow.ToString("yyyy/MM/dd"));
            Assert.That(Directory.Exists(expectedDirectory), Is.True, "저장 디렉토리가 생성되지 않았습니다.");

            string[] files = Directory.GetFiles(expectedDirectory);
            Assert.That(files.Length, Is.EqualTo(1), "이미지가 저장되지 않았습니다.");
            string savedFilePath = files[0];
            Assert.That(File.Exists(savedFilePath), Is.True, "저장된 파일이 존재하지 않습니다.");

            // DB 저장 호출 검증
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<VisionNg>()), Times.Once, "DB 저장이 호출되지 않았습니다.");
        }

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
}
