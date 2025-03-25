using NUnit.Framework;
using Magazine.WebApi.Controllers;
using Magazine.Core.Services;
using Magazine.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Magazine.Tests
{
    [TestFixture]
    public class TestsProductController
    {
        private Mock<IProductService> _mockProductService;
        private ProductController _controller;

        [SetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
        }

        [Test]
        public void Add_ValidProduct_ReturnsCreatedResult()
        {
            // Arrange
            var product = new Product 
            { 
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Definition = "Test Definition",
                Price = 10.99m,
                Image = "test.jpg"
            };
            
            _mockProductService.Setup(service => service.Add(It.IsAny<Product>()))
                .Returns(product);

            // Act
            var result = _controller.Add(product);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult.Value, Is.EqualTo(product));
        }

        [Test]
        public void GetById_ExistingProduct_ReturnsOkResult()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId };
            _mockProductService.Setup(service => service.Search(productId))
                .Returns(product);

            // Act
            var result = _controller.GetById(productId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(product));
        }

        [Test]
        public void GetById_NonexistentProduct_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductService.Setup(service => service.Search(productId))
                .Returns((Product)null);

            // Act
            var result = _controller.GetById(productId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public void GetAll_ReturnsOkResult()
        {
            // Arrange
            var products = new List<Product> 
            { 
                new Product { Id = Guid.NewGuid() },
                new Product { Id = Guid.NewGuid() }
            };
            _mockProductService.Setup(service => service.GetAll())
                .Returns(products);

            // Act
            var result = _controller.GetAll();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(products));
        }
    }
}