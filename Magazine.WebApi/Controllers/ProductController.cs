using Microsoft.AspNetCore.Mvc;
using Magazine.Core.Models;
using Magazine.Core.Services;
using System;

namespace Magazine.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Указывает на успешное создание
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Для ошибки 400
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Для ошибки 500
        [HttpPost]
        public IActionResult Add([FromBody] Product product)
        {
            try
            {
                // Теперь product.Image - это строка
                var createdProduct = _productService.Add(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Remove(Guid id)
        {
            try
            {
                var removedProduct = _productService.Remove(id);
                if (removedProduct == null)
                {
                    return NotFound();
                }
                return Ok(removedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Edit(Guid id, [FromBody] Product product)
        {
            if (product.Id != id)
            {
                return BadRequest("Product ID in URL does not match the ID in the product object.");
            }

            try
            {
                var updatedProduct = _productService.Edit(product);
                if (updatedProduct == null)
                {
                    return NotFound();
                }
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var products = _productService.GetAll(); // Предполагается, что метод GetAll реализован в вашем IProductService
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var product = _productService.Search(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
