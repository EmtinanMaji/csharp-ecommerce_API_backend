using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.EntityFramework;
using api.Model;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{

    public class ProductService
    {

        private AppDbContext appDbContext;
        public ProductService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;

        }
        public async Task<List<Product>> GetProducts()
        {

            return await appDbContext.Products
              .Include(product => product.Category)
              .Include(product => product.OrderItems)
              .ThenInclude(orderItem => orderItem.Product)
           .ToListAsync();//using appContext to return all product on table
        }

        public async Task<Product?> GetProductById(Guid ProductId)
        {
            return await appDbContext.Products.Include(Product => Product.Category)
            .Include(product => product.OrderItems)
                   .ThenInclude(orderItem => orderItem.Product)
            .FirstOrDefaultAsync(Product => Product.Id == ProductId);
        }
        public async Task<Product> CreateProductService(ProductModel NewProduct)
        {

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = NewProduct.Name,
                Slug = NewProduct.Slug,
                ImageUrl = NewProduct.ImageUrl,
                Description = NewProduct.Description,
                Price = NewProduct.Price,
                Quantity = NewProduct.Quantity,
                Sold = NewProduct.Sold,
                Shipping = NewProduct.Shipping,
                CreatedAt = NewProduct.CreatedAt
            };
            appDbContext.Products.Add(product);
            await appDbContext.SaveChangesAsync();
            return product;
        }
        public async Task AddProductOrder(Guid ProductId, Guid OrderId)
        {
            var orderItem = new OrderItem
            {
                OrderId = OrderId,
                ProductId = ProductId
            };

            await appDbContext.OrderItems.AddAsync(orderItem);
            await appDbContext.SaveChangesAsync();
        }
        public async Task<Product?> UpdateProductService(Guid ProductId, ProductModel updateProduct)
        {
            //     //create record 
            var productUpdated = appDbContext.Products
            .Include(product => product.Category)
            .Include(product => product.OrderItems)
            .ThenInclude(orderItem => orderItem.Product)
            .FirstOrDefault(product =>
            product.Id == ProductId);
            {
                if (productUpdated != null)
                {
                    productUpdated.Name = updateProduct.Name;
                    productUpdated.Slug = updateProduct.Slug;
                    productUpdated.ImageUrl = updateProduct.ImageUrl;
                    productUpdated.Description = updateProduct.Description;
                    productUpdated.Quantity = updateProduct.Quantity;
                    productUpdated.Sold = updateProduct.Sold;
                    productUpdated.Shipping = updateProduct.Shipping;
                    productUpdated.CategoryId = updateProduct.CategoryId;
                }
                appDbContext.SaveChanges();
                return productUpdated;
            }
        }
        public async Task<bool> DeleteProductService(Guid ProductId)
        {

            var ProductToRemove = appDbContext.Products.FirstOrDefault(P => P.Id == ProductId);
            if (ProductToRemove != null)
            {
                appDbContext.Products.Remove(ProductToRemove);
                await appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}