using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.DTOs;
using api.EntityFramework;
using api.Helpers;
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

        public async Task <PaginationDto<Product>> GetProducts(int pageNumber, int pageSize)

        {
            var totalProductCount = await appDbContext.Products.CountAsync();


            var products = await appDbContext.Products
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Category)
            .ToListAsync();
            return new PaginationDto<Product>
            {
                Items = products,
                TotalCount = totalProductCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

             //return await appDbContext.Products
               //.Include(product => product.Category)
               //.Include(product => product.OrderItems)
               //.ThenInclude(orderItem => orderItem.Product)
            //.ToListAsync();//using appContext to return all product on table

        }

        public async Task<Product?> GetProductById(Guid ProductId)
        {
            return await appDbContext.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(Product => Product.Id == ProductId);
        }
        public async Task<Product> CreateProductService(Product NewProduct)
        {
            Product product = new Product
        {

            Id = Guid.NewGuid(),
            Name = NewProduct.Name,
            Slug = Slug.GenerateSlug(NewProduct.Name),
            ImageUrl = NewProduct.ImageUrl,
            Description = NewProduct.Description,
            Quantity = NewProduct.Quantity,
            Sold = NewProduct.Sold,
            Shipping = NewProduct.Shipping,
            Price = NewProduct.Price,
            CategoryId = NewProduct.CategoryId,
            CreatedAt = DateTime.UtcNow

        };
            appDbContext.Products.Add(product);
            await appDbContext.SaveChangesAsync();
            return NewProduct;
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
                await appDbContext.SaveChangesAsync();
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

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
        {
            return await appDbContext.Products
               .Where(p => p.Name.Contains(keyword))
               .ToListAsync();
        }
    }
}