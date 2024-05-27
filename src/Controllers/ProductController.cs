using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.EntityFramework;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
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
        public async Task<IActionResult> GetProducts([FromQuery] QueryParameters queryParams)
        {
            try
            {
                var product = await _productService.GetProducts(queryParams);
                if (product == null)
                {
                    return ApiResponse.NotFound("No Product Found");
                }
                return ApiResponse.Success(product, "All products are returned successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            try
            {

                var product = await _productService.GetProductById(productId);
                if (product == null)
                {
                    return NotFound(new ErrorResponse { Message = $"There is no Product found with ID : {productId}" });
                }
                else
                {
                    return Ok(new SuccessResponse<Product> { Success = true, Message = "product is returned successfully", Data = product });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not return the Product");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(Product newProduct)
        {
            try
            {
                var createdProduct = await _productService.CreateProductService(newProduct);
                if (createdProduct != null)
                {
                    return CreatedAtAction(nameof(GetProduct), new { productId = createdProduct.ProductId }, createdProduct);
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

        // [HttpPost("AddOrderItem")]
        // public async Task<IActionResult> AddProductOrder([FromQuery] Guid ProductId, [FromQuery] Guid OrderId)
        // {
        //     try
        //     {
        //         await _productService.AddProductOrder(ProductId, OrderId);
        //         return ApiResponse.Created("created");
        //     }
        //     catch (Exception ex)
        //     {
        //         return ApiResponse.ServerError(ex.Message);
        //     }
        // }
        [HttpPut("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductService(Guid productId, Product updateProduct)
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
        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            try
            {
                var result = await _productService.DeleteProductService(productId);
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"The Product with ID : {productId} is not found to be deleted" });
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
        
    }
}