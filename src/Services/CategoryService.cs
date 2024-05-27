
using api.DTOs;
using api.EntityFramework;
using api.Helpers;
using Microsoft.EntityFrameworkCore;
namespace api.Services
{
    public class CategoryService
    {
        private AppDbContext _appDbContext;
        public CategoryService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        
    public async Task<PaginationDto<Category>> GetAllCategoryService(QueryParameters queryParams)
    {
            // Start with a base query
            var query = _appDbContext.Categories.AsQueryable();

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

            var totalCategoryCount = await query.CountAsync();

            var Categories = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();
            return new PaginationDto<Category>
            {
                Items = Categories,
                TotalCount = totalCategoryCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }
    public async Task<Category?> GetCategoryById(Guid categoryId)
    {
        return await _appDbContext.Categories.FirstOrDefaultAsync(category => category.CategoryId == categoryId);
    }
    public async Task<Category?> CreateCategoryService(Category newCategory)
    {
        newCategory.CategoryId = Guid.NewGuid();
        newCategory.Slug = Slug.GenerateSlug(newCategory.Name);
        newCategory.CreatedAt = DateTime.UtcNow;
        _appDbContext.Categories.Add(newCategory); 
        await _appDbContext.SaveChangesAsync();
        return newCategory;
    }
    public async Task<Category?> UpdateCategoryService(Guid categoryId, Category updateCategory)
    {
        var existingCategory = _appDbContext.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
        if (existingCategory != null)
            {
                existingCategory.Name = updateCategory.Name;
                existingCategory.Slug = Slug.GenerateSlug(existingCategory.Name);
                existingCategory.Description = updateCategory.Description;
                
            }
            await _appDbContext.SaveChangesAsync();
            return existingCategory;
        }
        public async Task<bool> DeleteCategoryService(Guid categoryId)
        {
            var categoryToRemove = _appDbContext.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (categoryToRemove != null)
            {
                _appDbContext.Categories.Remove(categoryToRemove);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}