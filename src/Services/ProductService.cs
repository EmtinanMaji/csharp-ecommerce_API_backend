using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using api.DTOs;
using api.EntityFramework;
using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{

    public class ProductService
    {
        private AppDbContext _appDbContext;
        public ProductService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }

        public async Task <PaginationDto<Product>> GetProducts(QueryParameters queryParams)

        {
            // Start with a base query
            var query = _appDbContext.Products.Include(p => p.Category).AsQueryable();

            // Apply search keyword filter
            if (!string.IsNullOrEmpty(queryParams.SearchKeyword))
            {
                query = query.Where(p => p.Name.ToLower().Contains(queryParams.SearchKeyword.ToLower()) || p.Description.ToLower().Contains(queryParams.SearchKeyword.ToLower()));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy))
            {
                query = queryParams.SortOrder == "desc" 
                ? query.OrderByDescending(u => EF.Property<object>(u, queryParams.SortBy))
                : query.OrderBy(u => EF.Property<object>(u, queryParams.SortBy));
            }

            // Apply category filter
            if (queryParams.SelectedCategories != null && queryParams.SelectedCategories.Any())
            {
                query = query.Where(p => queryParams.SelectedCategories.Contains(p.Category.CategoryId));
            }

            if (queryParams.MinPrice.HasValue){
                query = query.Where(p => p.Price >= queryParams.MinPrice.Value);
            }
            if (queryParams.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= queryParams.MaxPrice.Value);
            }

            var totalProductCount = await query.CountAsync();

            var products = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();
            return new PaginationDto<Product>
            {
                Items = products,
                TotalCount = totalProductCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<Product?> GetProductById(Guid productId)
        {
            return await _appDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == productId);
        }
        public async Task<Product> CreateProductService(Product newProduct)
        {
            newProduct.ProductId = Guid.NewGuid();
            newProduct.Slug = Slug.GenerateSlug(newProduct.Name);
            newProduct.CreatedAt = DateTime.UtcNow;
            _appDbContext.Products.Add(newProduct);
            await _appDbContext.SaveChangesAsync();
            return newProduct;

            // var product = new Product
            // {
            //     Id = Guid.NewGuid(),
            //     Name = newProduct.Name,
            //     Slug = newProduct.Slug,
            //     ImageUrl = newProduct.ImageUrl,
            //     Description = newProduct.Description,
            //     Price = newProduct.Price,
            //     Quantity = newProduct.Quantity,
            //     Sold = newProduct.Sold,
            //     Shipping = newProduct.Shipping,
            //     CreatedAt = newProduct.CreatedAt
            // };
            // _appDbContext.Products.Add(newProduct);
            // await _appDbContext.SaveChangesAsync();
            // return newProduct;
        }
        // public async Task AddProductOrder(Guid ProductId, Guid OrderId)
        // {
        //     var orderItem = new OrderItem
        //     {
        //         OrderId = OrderId,
        //         ProductId = ProductId
        //     };

        //     await _appDbContext.OrderItems.AddAsync(orderItem);
        //     await _appDbContext.SaveChangesAsync();
        // }
        public async Task<Product?> UpdateProductService(Guid productId, Product updateProduct)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return null;
            }

            // Optionally check if the category exists
            var categoryExists = await _appDbContext.Categories.AnyAsync(c => c.CategoryId == updateProduct.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Invalid category ID");
            }
            // Update the product fields if they are not null or empty
            product.Name = string.IsNullOrEmpty(updateProduct.Name) ? product.Name : updateProduct.Name;
            product.Slug = Slug.GenerateSlug(updateProduct.Name);
            product.ImageUrl = string.IsNullOrEmpty(updateProduct.ImageUrl) ? product.ImageUrl : updateProduct.ImageUrl;
            product.Description = string.IsNullOrEmpty(updateProduct.Description) ? product.Description : updateProduct.Description;
            product.Quantity = updateProduct.Quantity != default ? updateProduct.Quantity : product.Quantity;
            product.Sold = updateProduct.Sold != default ? updateProduct.Sold : product.Sold;
            product.Shipping = updateProduct.Shipping != default ? updateProduct.Shipping : product.Shipping;
            product.CategoryId = updateProduct.CategoryId != default ? updateProduct.CategoryId : product.CategoryId;
            

            _appDbContext.Products.Update(product);
            await _appDbContext.SaveChangesAsync();
            return product;
        
    }
        public async Task<bool> DeleteProductService(Guid productId)
        {

            var productToRemove = _appDbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (productToRemove != null)
            {
                _appDbContext.Products.Remove(productToRemove);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}