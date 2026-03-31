using MarfinaLibrary.Services;
using System;
using Xunit;

namespace MarfinaUnitTest
{
    /// <summary>
    /// Тесты для DiscountService
    /// </summary>
    public class DiscountServiceTests
    {
        private readonly DiscountService _service;

        public DiscountServiceTests()
        {
            _service = new DiscountService();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(9999.99)]
        public void CalculateDiscount_LessThan10000_Returns0(decimal totalSales)
        {
            // Act
            var discount = _service.CalculateDiscount(totalSales);

            // Assert
            Assert.Equal(0, discount);
        }

        [Theory]
        [InlineData(10000)]
        [InlineData(25000)]
        [InlineData(49999.99)]
        public void CalculateDiscount_Between10000And50000_Returns5(decimal totalSales)
        {
            // Act
            var discount = _service.CalculateDiscount(totalSales);

            // Assert
            Assert.Equal(5, discount);
        }

        [Theory]
        [InlineData(50000)]
        [InlineData(150000)]
        [InlineData(299999.99)]
        public void CalculateDiscount_Between50000And300000_Returns10(decimal totalSales)
        {
            // Act
            var discount = _service.CalculateDiscount(totalSales);

            // Assert
            Assert.Equal(10, discount);
        }

        [Theory]
        [InlineData(300000)]
        [InlineData(500000)]
        [InlineData(1000000)]
        public void CalculateDiscount_MoreThan300000_Returns15(decimal totalSales)
        {
            // Act
            var discount = _service.CalculateDiscount(totalSales);

            // Assert
            Assert.Equal(15, discount);
        }

       

        [Fact]
        public void CalculateDiscount_EdgeCases_ReturnsCorrectValues()
        {
            // Arrange & Act & Assert
            Assert.Equal(0, _service.CalculateDiscount(9999.99m));
            Assert.Equal(5, _service.CalculateDiscount(10000m));
            Assert.Equal(5, _service.CalculateDiscount(49999.99m));
            Assert.Equal(10, _service.CalculateDiscount(50000m));
            Assert.Equal(10, _service.CalculateDiscount(299999.99m));
            Assert.Equal(15, _service.CalculateDiscount(300000m));
        }
    }
}