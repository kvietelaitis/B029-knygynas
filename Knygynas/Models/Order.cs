using System.ComponentModel.DataAnnotations;

namespace Knygynas.Models;

public class Order
{
    public int Id { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public double TotalPrice { get; set; }
    
    [Required]
    public string DeliveryMethod { get; set; } = string.Empty; // Postomat, Bookstore, Address
    
    [Required]
    public string FromCity { get; set; } = string.Empty;
    
    [Required]
    public string FromAddress { get; set; } = string.Empty;
    
    [Required]
    public string ToCity { get; set; } = string.Empty;
    
    [Required]
    public string ToAddress { get; set; } = string.Empty;
    
    public OrderState State { get; set; } = OrderState.PendingPayment;
    
    public List<OrderItem> OrderItems { get; set; } = new();
    
    public string? UserId { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public string? StripeSessionId { get; set; }
}
