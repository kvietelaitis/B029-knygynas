namespace Knygynas.Models;

public class CartItem
{
    public string ISBN { get; set; }
    public string Title { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public double Total => Price * Quantity;
}
