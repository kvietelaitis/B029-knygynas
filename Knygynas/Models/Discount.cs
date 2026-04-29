namespace Knygynas.Models;

public class Discount
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public ICollection<Book> Books { get; set; }
}