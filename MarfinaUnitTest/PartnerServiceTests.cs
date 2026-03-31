using MarfinaLibrary.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MarfinaUnitTest
{
    public class PartnerServiceTests
    {
        [Fact]
        public void Constructor_CreatesInstance()
        {
            // Arrange & Act
            var service = new PartnerService();

            // Assert
            Assert.NotNull(service);
        }


        [Fact]
        public async Task GetPartnerWithInvalidId_ThrowsException()
        {
            // Arrange
            var service = new PartnerService();
            int invalidId = -999;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetPartnerWithDiscountAsync(invalidId));
        }

        
    }
}