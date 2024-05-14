using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public CategoryController(AppDbContext appDbContext)
        {
            _categoryService = new CategoryService(appDbContext);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoryService();
                if (categories.ToList().Count <= 0)
                {
                    return NotFound(new ErrorResponse { Message = "There is no categories to display" });
                }
                else
                {
                    return Ok(new SuccessResponse<IEnumerable<Category>>
                    {
                        Message = "Categories are returned successfully",
                        Data = categories
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not return the category list");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
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
                    return NotFound(new ErrorResponse { Message = $"There is no category found with ID : {categoryId}" });
                }
                else
                {
                    return Ok(new SuccessResponse<Category>
                    {
                        Message = "Category is returned successfully",
                        Data = category
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There is an error , can not return the category");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(Category newCategory)
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
                Console.WriteLine($"There is an error , can not create new category");
                return StatusCode(500, new ErrorResponse { Message = ex.Message });
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