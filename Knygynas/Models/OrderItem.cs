using System.ComponentModel.DataAnnotations;

namespace Knygynas.Models;

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    
    [Required]
    public string BookISBN { get; set; } = string.Empty;
    public Book? Book { get; set; }
    
    public double Price { get; set; }
    
    public int Quantity { get; set; }
}
