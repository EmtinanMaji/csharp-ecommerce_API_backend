using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.Models;



namespace api.DTOs
{
  public class CategoryDto
  {
    
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;

    // Relationships
    public List<ProductModel>? Products { get; set; } 
  }

}