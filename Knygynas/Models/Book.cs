using System.ComponentModel.DataAnnotations;

namespace Knygynas.Models;

public class Book
{
    [Key]
    public string ISBN { get; set; }
    
    public string Title { get; set; }
    public bool Available { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int PageCount { get; set; }
    public string Language { get; set; }
    public string EANCode { get; set; }
    public float? Length { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public double Price { get; set; }
    
    public int DiscountId { get; set; }
    public Discount Discount { get; set; }
    
    public ICollection<AuthorRole> AuthorRoles { get; set; }
    
    public Book()
    {
        AuthorRoles = new List<AuthorRole>();
    }
}