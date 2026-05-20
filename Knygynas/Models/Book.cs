using System.ComponentModel.DataAnnotations;

namespace Knygynas.Models;

public class Book
{
    [Key]
    public string ISBN { get; set; }
    
    public string Title { get; set; }
    public int Quantity { get; set; }
    public bool Available => Quantity > 0;
    public DateTime ReleaseDate { get; set; }
    public int PageCount { get; set; }
    public string Language { get; set; }
    public string EANCode { get; set; }
    public string? CoverImageUrl { get; set; }
    public float? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public double Price { get; set; }
    
    public int DiscountId { get; set; }
    public Discount Discount { get; set; }
    
    public ICollection<AuthorRole> AuthorRoles { get; set; }
    
    public ICollection<BookCategory> BookCategories { get; set; }
    
    public Book()
    {
        AuthorRoles = new List<AuthorRole>();
        BookCategories = new List<BookCategory>();
    }
}