using MarfinaLibrary.Services;
using System.Threading.Tasks;
using Xunit;

namespace MarfinaUnitTest
{
    public class SalesServiceTests
    {
        [Fact]
        public void SalesService_Constructor_CreatesInstance()
        {
            // Arrange & Act
            var service = new SalesService();

            // Assert
            Assert.NotNull(service);
        }

       
    }
}