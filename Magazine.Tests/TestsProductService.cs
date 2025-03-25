using NUnit.Framework;
using Magazine.WebApi.Services;
using Magazine.Core.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;

namespace Magazine.Tests
{
    [TestFixture]
    public class TestsProductService
    {
        private IConfiguration _configuration;
        private ProductService _productService;
        private string _testDbPath;

        [SetUp]
        public void Setup()
        {
            _testDbPath = Path.Combine(Path.GetTempPath(), $"test_products_{Guid.NewGuid()}.json");
            
            var inMemorySettings = new Dictionary<string, string> {
                {"DataBaseFilePath", _testDbPath}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _productService = new ProductService(_configuration);
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
        public void Add_ValidProduct_ReturnsAddedProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Definition = "Test Definition",
                Price = 10.99m,
                Image = "test.jpg"
            };

            // Act
            var result = _productService.Add(product);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Name, Is.EqualTo(product.Name));
        }

        [Test]
        public void GetAll_ReturnsAllProducts()
        {
            // Arrange
            var product1 = new Product { Name = "Product 1" };
            var product2 = new Product { Name = "Product 2" };
            
            _productService.Add(product1);
            _productService.Add(product2);

            // Act
            var results = _productService.GetAll();

            // Assert
            Assert.That(results, Has.Count.EqualTo(2));
        }

        [Test]
        public void Search_ExistingProduct_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product" };
            var addedProduct = _productService.Add(product);

            // Act
            var result = _productService.Search(addedProduct.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(addedProduct.Id));
        }

        [Test]
        public void Remove_ExistingProduct_RemovesProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product" };
            var addedProduct = _productService.Add(product);

            // Act
            var removedProduct = _productService.Remove(addedProduct.Id);
            var searchResult = _productService.Search(addedProduct.Id);

            // Assert
            Assert.That(removedProduct, Is.Not.Null);
            Assert.That(searchResult, Is.Null);
        }
    }
}