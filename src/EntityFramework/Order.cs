using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.EntityFramework
{
    [Table("Orders")]
    public class Order
    {
        [Key, Required] // validation
        public Guid OrderId { get; set; }
        [Required]
        public Guid UserId { get; set; }// Foreign Key

        [Required]
        [MaxLength(30)]
        public required string OrderStatus { get; set; }
        [Required]
        public int OrderTotal { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        //public Guid ProductId { get; set; }// Foreign Key
        //public Product Product { get; set; } // get Product Entity

        //Relationships
        public User? User { get; set; } // get User Entity
        public List<OrderItem>? OrderItems { get; set; } // for M-M with product
    }
}