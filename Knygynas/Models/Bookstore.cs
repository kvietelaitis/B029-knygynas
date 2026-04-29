namespace Knygynas.Models;

public class Bookstore
{
    public int Id { get; set; }
    
    public string City { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
