using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.EntityFramework
{
    [Table("Categories")]
    public class Category
    {
        [Key, Required]
        [Column("category_id")]
        public Guid CategoryId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Column("slug")]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(300)]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        // Relationships
        public ICollection<Product>? Products { get; set; }

    }

}