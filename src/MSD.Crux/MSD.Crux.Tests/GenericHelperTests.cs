using MSD.Crux.Core.Helpers;
using MSD.Crux.Core.Models;
using NUnit.Framework;

namespace MSD.Crux.Tests.Helpers
{
    [TestFixture]
    public class GenericHelperTests
    {
        [Test]
        public void ConvertToEmployeeNumber_ShouldCombineCorrectly()
        {
            // Arrange
            short year = 2025;
            MillenniumGender gender = MillenniumGender.Female;
            short sequence = 5;

            // Act
            int result = GenericHelper.ConvertToEmployeeNumber(year, gender, sequence);

            // Assert
            Assert.That(result, Is.EqualTo(202520005), "Employee Number가 예상 값과 다릅니다.");
        }
    }
}
