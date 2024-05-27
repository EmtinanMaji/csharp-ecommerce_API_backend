using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using api.EntityFramework;
using api.Models;

namespace api.Models
{
  public class OrderItemModel
  {
    
    public Guid OrderId { get; set; }//Foreign Key
    
    [Column("product_id")]
    public Guid ProductId { get; set; }//Foreign Key
   
    public int Quantity { get; set; }
    //Relations M-M between Orders and Products
    public OrderModel? Order { get; set; }
    public ProductModel? Product { get; set; }
  }
}