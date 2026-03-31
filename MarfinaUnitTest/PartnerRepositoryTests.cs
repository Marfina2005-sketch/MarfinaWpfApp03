using MarfinaLibrary.Models;
using MarfinaLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MarfinaUnitTest
{
    public class PartnerRepositoryTests : IDisposable
    {
        private readonly PartnerRepository _repository;

        public PartnerRepositoryTests()
        {
            _repository = new PartnerRepository();
        }

        [Fact]
        public void Constructor_CreatesInstance()
        {
            // Arrange & Act
            using (var repo = new PartnerRepository())
            {
                // Assert
                Assert.NotNull(repo);
            }
        }


        [Fact]
        public void Dispose_Works()
        {
            _repository.Dispose();
            Assert.True(true); 
        }



        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
    }
}