using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs;
using api.EntityFramework;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("/api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly AuthService _authService;
        public CategoryController(CategoryService categoryService, AuthService authService)
        {
            _categoryService = categoryService;
            _authService = authService;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory([FromQuery] QueryParameters queryParams)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoryService(queryParams);
                if (categories == null)
                {
                    return ApiResponse.NotFound("There is no categories to display");
                }
                return ApiResponse.Success(categories, "Categories are returned successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategory(Guid categoryId)
        {
            
            try
            {

                var category = await _categoryService.GetCategoryById(categoryId);
                if (category == null)
                {
                    return ApiResponse.NotFound($"There is no category found with ID : {categoryId}");
                }
                else
                {
                    return ApiResponse.Success(category, "Category is returned successfully");
                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] Category newCategory)
        {
            try
            {
                var createdCategory = await _categoryService.CreateCategoryService(newCategory);
                if (createdCategory != null)
                {
                    return CreatedAtAction(nameof(GetCategory), new { categoryId = createdCategory.CategoryId }, createdCategory);
                }
                else
                {
                    return Ok(new SuccessResponse<Category>
                    {
                        Message = "Category is created successfully",
                        Data = createdCategory
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, Category updateCategory)
        {
            try
            {

                var category = await _categoryService.UpdateCategoryService(categoryId, updateCategory);
                if (category == null)
                {
                    return NotFound(new ErrorResponse { Message = "There is no category found to update." });
                }
                else
                {
                    return Ok(new SuccessResponse<Category>
                    {
                        Message = "Category is updated  succeefully",
                        Data = category
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not update the category");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }
        }


        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            try
            {

                var result = await _categoryService.DeleteCategoryService(categoryId);
                if (!result)
                {
                    return NotFound(new ErrorResponse { Message = $"The category with ID : {categoryId} is not found to be deleted" });
                }
                else
                {
                    return Ok(new SuccessResponse<Category>
                    {
                        Message = "Category is deleted succeefully",
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not delete the category");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }

        }

    }
}