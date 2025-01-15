using MSD.Crux.Core.Models.TCP;
using NUnit.Framework;

namespace MSD.Crux.Tests.Models;

[TestFixture]
public class VpbusFrameHeaderTests
{
    [Test]
    [TestCase(new byte[] { 0, 50, 0, 1, 1, 0 }, FrameType.Jwt, 50, 1, 1, 0, TestName = "ParseHeader_ValidJwtFrame")]
    [TestCase(new byte[] { 1, 50, 0, 1, 1, 0 }, FrameType.Injection, 50, 1, 1, 0, TestName = "ParseHeader_ValidInjectionFrame")]
    [TestCase(new byte[] { 2, 50, 0, 1, 1, 0 }, FrameType.Vision, 50, 1, 1, 0, TestName = "ParseHeader_ValidVisionFrame")]
    public void ParseHeader_ValidData_Success(byte[] headerData, FrameType expectedFrameType, int expectedPayloadLength, byte expectedDataVersion, byte expectedRole,
                                              byte expectedReserved)
    {
        // Arrange
        VpbusFrameHeader frameHeader = new();

        // Act
        frameHeader.FrameType = (FrameType)headerData[0];
        frameHeader.PayloadLength = BitConverter.ToUInt16(headerData, 1);
        frameHeader.DataVersion = headerData[3];
        frameHeader.Role = headerData[4];
        frameHeader.Reserved = headerData[5];

        // Assert
        Assert.That(frameHeader.FrameType, Is.EqualTo(expectedFrameType));
        Assert.That(frameHeader.PayloadLength, Is.EqualTo(expectedPayloadLength));
        Assert.That(frameHeader.DataVersion, Is.EqualTo(expectedDataVersion));
        Assert.That(frameHeader.Role, Is.EqualTo(expectedRole));
        Assert.That(frameHeader.Reserved, Is.EqualTo(expectedReserved));
    }

    [Test]
    [TestCase(new byte[] { 255, 0, 0, 0, 0, 0 }, TestName = "FrameType_InvalidValue_ThrowsException")]
    public void ParseHeader_InvalidFrameType_ThrowsException(byte[] headerData)
    {
        // Arrange
        VpbusFrameHeader frameHeader = new();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
                                                 {
                                                     if (Enum.IsDefined(typeof(FrameType), (int)headerData[0]))
                                                     {
                                                         frameHeader.FrameType = (FrameType)headerData[0];
                                                     }
                                                     else
                                                     {
                                                         throw new InvalidOperationException($"Invalid FrameType: {headerData[0]}");
                                                     }
                                                 });
    }
}
