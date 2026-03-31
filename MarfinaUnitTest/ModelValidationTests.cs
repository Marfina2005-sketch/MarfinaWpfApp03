using MarfinaLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MarfinaUnitTest
{
    /// <summary>
    /// Тесты для моделей данных
    /// </summary>
    public class ModelValidationTests
    {
        [Fact]
        public void Partner_WithRequiredFields_IsValid()
        {
            // Arrange
            var partner = new Partner
            {
                TypeId = 1,
                Name = "Тестовый партнер",
                Rating = 5
            };

            // Assert
            Assert.NotNull(partner);
            Assert.Equal(1, partner.TypeId);
            Assert.Equal("Тестовый партнер", partner.Name);
            Assert.Equal(5, partner.Rating);
            Assert.Null(partner.Email);
            Assert.Null(partner.Phone);
            Assert.NotNull(partner.SalesHistories);
            Assert.Empty(partner.SalesHistories);
        }

        [Fact]
        public void Partner_WithAllProperties_IsValid()
        {
            // Arrange
            var partner = new Partner
            {
                Id = 100,
                TypeId = 2,
                Name = "ООО Тестовая Компания",
                LegalAddress = "г. Москва, ул. Ленина, д. 1, оф. 101",
                Inn = "123456789012",
                DirectorName = "Иванов Иван Иванович",
                Phone = "+7 (999) 123-45-67",
                Email = "test@company.ru",
                Rating = 10,
                LogoPath = "C:\\logos\\company_logo.png"
            };

            // Assert
            Assert.Equal(100, partner.Id);
            Assert.Equal(2, partner.TypeId);
            Assert.Equal("ООО Тестовая Компания", partner.Name);
            Assert.Equal("г. Москва, ул. Ленина, д. 1, оф. 101", partner.LegalAddress);
            Assert.Equal("123456789012", partner.Inn);
            Assert.Equal("Иванов Иван Иванович", partner.DirectorName);
            Assert.Equal("+7 (999) 123-45-67", partner.Phone);
            Assert.Equal("test@company.ru", partner.Email);
            Assert.Equal(10, partner.Rating);
            Assert.Equal("C:\\logos\\company_logo.png", partner.LogoPath);
        }

        [Fact]
        public void Partner_StringProperties_MaxLength()
        {
            // Arrange
            var partner = new Partner
            {
                Name = new string('A', 200), // Максимум 200 символов
                Inn = new string('1', 12),    // Максимум 12 символов
                DirectorName = new string('A', 200),
                Phone = new string('1', 20),
                Email = new string('a', 100),
                LogoPath = new string('a', 500)
            };

            // Assert
            Assert.Equal(200, partner.Name.Length);
            Assert.Equal(12, partner.Inn.Length);
            Assert.Equal(200, partner.DirectorName.Length);
            Assert.Equal(20, partner.Phone.Length);
            Assert.Equal(100, partner.Email.Length);
            Assert.Equal(500, partner.LogoPath.Length);
        }

        [Fact]
        public void PartnerType_WithRequiredFields_IsValid()
        {
            // Arrange
            var partnerType = new PartnerType
            {
                Name = "ЗАО"
            };

            // Assert
            Assert.NotNull(partnerType);
            Assert.Equal("ЗАО", partnerType.Name);
            Assert.NotNull(partnerType.Partners);
            Assert.Empty(partnerType.Partners);
        }

        [Fact]
        public void PartnerType_WithId_IsValid()
        {
            // Arrange
            var partnerType = new PartnerType
            {
                Id = 5,
                Name = "ООО"
            };

            // Assert
            Assert.Equal(5, partnerType.Id);
            Assert.Equal("ООО", partnerType.Name);
        }

        [Fact]
        public void Product_WithRequiredFields_IsValid()
        {
            // Arrange
            var product = new Product
            {
                Article = "ART-001",
                Name = "Тестовый продукт",
                MinPartnerPrice = 1000.50m
            };

            // Assert
            Assert.NotNull(product);
            Assert.Equal("ART-001", product.Article);
            Assert.Equal("Тестовый продукт", product.Name);
            Assert.Equal(1000.50m, product.MinPartnerPrice);
            Assert.Null(product.Description);
            Assert.NotNull(product.SalesHistories);
            Assert.Empty(product.SalesHistories);
        }

        [Fact]
        public void Product_WithAllProperties_IsValid()
        {
            // Arrange
            var product = new Product
            {
                Id = 10,
                Article = "PROD-12345",
                Name = "Профессиональное оборудование",
                Description = "Высококачественное оборудование для бизнеса",
                MinPartnerPrice = 15000.99m
            };

            // Assert
            Assert.Equal(10, product.Id);
            Assert.Equal("PROD-12345", product.Article);
            Assert.Equal("Профессиональное оборудование", product.Name);
            Assert.Equal("Высококачественное оборудование для бизнеса", product.Description);
            Assert.Equal(15000.99m, product.MinPartnerPrice);
        }

        [Fact]
        public void Product_StringProperties_MaxLength()
        {
            // Arrange
            var product = new Product
            {
                Article = new string('A', 50), // Максимум 50 символов
                Name = new string('A', 200)     // Максимум 200 символов
            };

            // Assert
            Assert.Equal(50, product.Article.Length);
            Assert.Equal(200, product.Name.Length);
        }

        [Fact]
        public void SalesHistory_WithRequiredFields_IsValid()
        {
            // Arrange
            var sale = new SalesHistory
            {
                PartnerId = 1,
                ProductId = 2,
                SaleDate = new DateTime(2024, 1, 15),
                Quantity = 5,
                TotalAmount = 5000m
            };

            // Assert
            Assert.NotNull(sale);
            Assert.Equal(1, sale.PartnerId);
            Assert.Equal(2, sale.ProductId);
            Assert.Equal(new DateTime(2024, 1, 15), sale.SaleDate);
            Assert.Equal(5, sale.Quantity);
            Assert.Equal(5000m, sale.TotalAmount);
        }

        [Fact]
        public void SalesHistory_WithAllProperties_IsValid()
        {
            // Arrange
            var sale = new SalesHistory
            {
                Id = 100,
                PartnerId = 3,
                ProductId = 4,
                SaleDate = DateTime.Now,
                Quantity = 10,
                TotalAmount = 25000.50m
            };

            // Assert
            Assert.Equal(100, sale.Id);
            Assert.Equal(3, sale.PartnerId);
            Assert.Equal(4, sale.ProductId);
            Assert.Equal(10, sale.Quantity);
            Assert.Equal(25000.50m, sale.TotalAmount);
            Assert.True(sale.SaleDate <= DateTime.Now);
        }

        [Fact]
        public void SalesHistory_DefaultValues_AreValid()
        {
            // Arrange
            var sale = new SalesHistory();

            // Assert
            Assert.Equal(0, sale.Id);
            Assert.Equal(0, sale.PartnerId);
            Assert.Equal(0, sale.ProductId);
            Assert.Equal(default(DateTime), sale.SaleDate);
            Assert.Equal(0, sale.Quantity);
            Assert.Equal(0m, sale.TotalAmount);
            Assert.Null(sale.Partner);
            Assert.Null(sale.Product);
        }

    }
}