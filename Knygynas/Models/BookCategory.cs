namespace Knygynas.Models;

public class BookCategory
{
    public string BookISBN { get; set; }
    public Book Book { get; set; }
    public int CategoryID { get; set; }
    public Category Category { get; set; }
}