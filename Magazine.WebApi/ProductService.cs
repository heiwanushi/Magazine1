using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Magazine.Core.Models;
using Magazine.Core.Services;

namespace Magazine.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<Guid, Product> _products;
        private readonly string _databasePath;
        private static readonly Mutex _mutex = new Mutex();

        public ProductService(IConfiguration configuration)
        {
            _configuration = configuration;
            _databasePath = _configuration["DataBaseFilePath"];
            _products = new Dictionary<Guid, Product>();
            
            var directory = Path.GetDirectoryName(_databasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_databasePath))
            {
                File.WriteAllText(_databasePath, "[]");
            }

            InitFromFile();
        }

        // Явная реализация метода Add
        Product IProductService.Add(Product product)
        {
            _mutex.WaitOne();
            try
            {
                if (product.Id == Guid.Empty)
                {
                    product.Id = Guid.NewGuid();
                }
                _products[product.Id] = product;
                WriteToFile();
                return product;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error adding product", ex);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        // Явная реализация метода Remove
        Product IProductService.Remove(Guid id)
        {
            _mutex.WaitOne();
            try
            {
                if (_products.TryGetValue(id, out Product product))
                {
                    _products.Remove(id);
                    WriteToFile();
                    return product;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error removing product", ex);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        // Явная реализация метода Edit
        Product IProductService.Edit(Product product)
        {
            _mutex.WaitOne();
            try
            {
                if (_products.ContainsKey(product.Id))
                {
                    _products[product.Id] = product;
                    WriteToFile();
                    return product;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error editing product", ex);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        // Явная реализация метода Search
        Product IProductService.Search(Guid id)
        {
            try
            {
                return _products.TryGetValue(id, out Product product) ? product : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error searching product", ex);
            }
        }

        // Явная реализация метода GetAll
        List<Product> IProductService.GetAll()
        {
            try
            {
                return _products.Values.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error getting all products", ex);
            }
        }

        private void InitFromFile()
        {
            _mutex.WaitOne();
            try
            {
                if (File.Exists(_databasePath))
                {
                    string jsonString = File.ReadAllText(_databasePath);
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var products = JsonSerializer.Deserialize<List<Product>>(jsonString);
                        _products.Clear();
                        foreach (var product in products)
                        {
                            _products[product.Id] = product;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error initializing from file", ex);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private void WriteToFile()
        {
            try
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                };
                string jsonString = JsonSerializer.Serialize(_products.Values, options);
                
                string tempPath = _databasePath + ".tmp";
                File.WriteAllText(tempPath, jsonString);
                
                if (File.Exists(_databasePath))
                {
                    File.Delete(_databasePath);
                }
                File.Move(tempPath, _databasePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error writing to file", ex);
            }
        }
    }
}