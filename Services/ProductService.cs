// --- FINALIZED IMPLEMENTATION: Services/ProductService.cs ---
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS_Group03.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetProductsWithSuppliersAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetProductWithSupplierAsync(id);
        }

        public async Task<Product?> GetProductWithSupplierAsync(int id)
        {
            return await GetProductByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (!await IsSkuUniqueAsync(product.Sku))
            {
                throw new InvalidOperationException($"Product with SKU '{product.Sku}' already exists.");
            }

            product.DateCreated = DateTime.UtcNow;
            product.LastUpdated = DateTime.UtcNow;

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.Id} not found.");
            }
            if (!await IsSkuUniqueAsync(product.Sku, product.Id))
            {
                throw new InvalidOperationException($"Another product with SKU '{product.Sku}' already exists.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Sku = product.Sku;
            existingProduct.Description = product.Description ?? string.Empty;
            existingProduct.Price = product.Price;
            existingProduct.LowStockThreshold = product.LowStockThreshold;
            existingProduct.SupplierId = product.SupplierId;
            existingProduct.LastUpdated = DateTime.UtcNow;

            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            if (product.QuantityInStock > 0)
            {
                throw new InvalidOperationException($"Cannot delete product '{product.Name}' as it has stock. Adjust to zero first.");
            }

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
        {
            return await _unitOfWork.Products.GetLowStockProductsAsync(threshold);
        }

        public async Task<bool> IsSkuUniqueAsync(string sku, int? productId = null)
        {
            if (string.IsNullOrWhiteSpace(sku)) return false;
            bool skuExists = await _unitOfWork.Products.SkuExistsAsync(sku, productId);
            return !skuExists;
        }
    }
}