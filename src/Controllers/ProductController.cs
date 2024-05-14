using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.EntityFramework;
using api.Model;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.Controller
{

    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(AppDbContext appDbContext)
        {
            _productService = new ProductService(appDbContext);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {

            var Product = await _productService.GetProducts(pageNumber, pageSize);
            if (Product == null)
        {
            return ApiResponse.NotFound("No Product Found");
        }
            return ApiResponse.Success(Product, "All products are returned successfully");

        }

        [HttpGet("{ProductId}")]
        public async Task<IActionResult> GetProduct(Guid ProductId)
        {
            try
            {

                var ProductById = await _productService.GetProductById(ProductId);
                if (ProductById == null)
                {
                    return NotFound(new ErrorResponse { Message = $"There is no Product found with ID : {ProductId}" });
                }
                else
                {
                    return ApiResponse.Success(ProductById, "All Product are returned successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not return the Product");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product NewProduct)
        {

            try
            {
                var createdProduct = await _productService.CreateProductService(NewProduct);
                if (createdProduct != null)
                {
                    return CreatedAtAction(nameof(GetProduct), new { productId = createdProduct.Id }, createdProduct);
                }
                else
                {
                    return Ok(new SuccessResponse<Product>
                    {
                        Message = "Product is created successfully",
                        Data = createdProduct
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"product can not be created");
                return StatusCode(500, new ErrorResponse { Success = false, Message = ex.Message });
            }
        }
        [HttpPost("AddOrderItem")]
        public async Task<IActionResult> AddProductOrder([FromQuery] Guid ProductId, [FromQuery] Guid OrderId)
        {
            try
            {
                await _productService.AddProductOrder(ProductId, OrderId);
                return ApiResponse.Created("created");
            }
            catch (Exception ex)
            {
                return ApiResponse.ServerError(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProductService(Guid productId,ProductModel updateProduct)
        {
            try
            {

                var product = await _productService.UpdateProductService(productId, updateProduct);
                if (product == null)
                {
                    return NotFound(new ErrorResponse { Message = "There is no product found to update." });
                }
                else
                {
                    return Ok(new SuccessResponse<Product> { Success = true, Message = "product is updated successfully", Data = product });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not update the product");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }
        [HttpDelete("{ProductId}")]
        public async Task<IActionResult> DeleteProduct(Guid ProductId)
        {
            try
            {

                var result = await _productService.DeleteProductService(ProductId);
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"The Product with ID : {ProductId} is not found to be deleted" });
                }
                else
                {
                    return Ok(new SuccessResponse<Product>
                    {
                        Message = "Product is deleted successfully",
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not delete the Product");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }

        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required for search.");
            }

            var products = await _productService.SearchProductsAsync(keyword);
            return Ok(products);
        }
        /*
        [HttpGet("products/search")]
    public async Task<IActionResult> SearchProducts(string? keyword, decimal? minPrice, decimal? maxPrice, string? sortBy, bool isAscending, int page = 1, int pageSize = 3)
    {
        var products = await _productService.SearchProductsAsync(keyword, minPrice, maxPrice, sortBy, isAscending, page, pageSize);
        if (products.Any())
        {
            return Ok(products);
        }
        else
        {
            throw new NotFoundException("No products found matching the search keyword");
        }
    }*/

    }
}