using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.EntityFramework;


namespace api.Models
{
  public class CategoryModel
  {
    [Key]
    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Column("name")]
    [Required(ErrorMessage = "Category name is required")]
    [MinLength(2, ErrorMessage = "Category name must be at least 2 characters long.")]
    [MaxLength(50, ErrorMessage = "Category name must be at most 50 characters long.")]
    public string Name { get; set; }

    [Column("slug")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(300, ErrorMessage = "Description can be at most 300 characters long.")]

    [Column("description")]    
    public string Description { get; set; } = string.Empty;

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; }

    // Relationships
    public ICollection<ProductModel>? Products { get; set; } 
  }

}