using Magazine.Core.Models;
using Magazine.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private static readonly List<Product> _products = new List<Product>();

        public Product Add(Product product)
        {
            try
            {
                _products.Add(product);
                return product;
            }
            catch (Exception ex)
            {
                // Логирование и обработка исключения
                throw new InvalidOperationException("Error adding product", ex);
            }
        }

        public Product Remove(Guid id)
        {
            try
            {
                var product = _products.FirstOrDefault(p => p.Id == id);
                if (product == null) return null;

                _products.Remove(product);
                return product;
            }
            catch (Exception ex)
            {
                // Логирование и обработка исключения
                throw new InvalidOperationException("Error removing product", ex);
            }
        }

        public Product Edit(Product product)
        {
            try
            {
                var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct == null) return null;

                existingProduct.Name = product.Name;
                existingProduct.Definition = product.Definition;
                existingProduct.Price = product.Price;
                existingProduct.Image = product.Image;

                return existingProduct;
            }
            catch (Exception ex)
            {
                // Логирование и обработка исключения
                throw new InvalidOperationException("Error editing product", ex);
            }
        }

        public Product Search(Guid id)
        {
            try
            {
                return _products.FirstOrDefault(p => p.Id == id);
            }
            catch (Exception ex)
            {
                // Логирование и обработка исключения
                throw new InvalidOperationException("Error searching for product", ex);
            }
        }
        public List<Product> GetAll()
        {
            return _products; // Возвращаем все продукты из списка
        }

    }
}
