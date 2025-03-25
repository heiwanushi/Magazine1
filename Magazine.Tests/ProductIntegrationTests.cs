using NUnit.Framework;
using Magazine.WebApi.Controllers;
using Magazine.WebApi.Services;
using Magazine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Magazine.Tests
{
    [TestFixture]
    public class ProductIntegrationTests
    {
        private ProductController _controller;
        private ProductService _productService;
        private IConfiguration _configuration;
        private string _testDbPath;

        [SetUp]
        public void Setup()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"integration_test_products_{Guid.NewGuid()}.json");
            
            var inMemorySettings = new Dictionary<string, string> {
                {"DataBaseFilePath", _testDbPath}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _productService = new ProductService(_configuration);
            _controller = new ProductController(_productService);
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(_testDbPath))
            {
                File.Delete(_testDbPath);
            }
        }

        [Test]
        public void IntegrationTest_AddAndRetrieveProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Integration Test Product",
                Definition = "Test Definition",
                Price = 99.99m,
                Image = "test.jpg"
            };

            // Act - Add product
            var addResult = _controller.Add(product) as CreatedAtActionResult;
            var addedProduct = addResult.Value as Product;

            // Act - Retrieve product
            var getResult = _controller.GetById(addedProduct.Id) as OkObjectResult;
            var retrievedProduct = getResult.Value as Product;

            // Assert
            Assert.That(retrievedProduct, Is.Not.Null);
            Assert.That(retrievedProduct.Id, Is.EqualTo(addedProduct.Id));
            Assert.That(retrievedProduct.Name, Is.EqualTo(product.Name));
            Assert.That(retrievedProduct.Price, Is.EqualTo(product.Price));
        }
    }
}