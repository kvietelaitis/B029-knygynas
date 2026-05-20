using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knygynas.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string WrittenReview { get; set; }

    public DateTime CreationDate { get; set; } = DateTime.UtcNow;

    // Foreign Key matching the type of Book.ISBN
    [Required]
    public string BookISBN { get; set; }

    // Navigation property back to the Book
    [ForeignKey(nameof(BookISBN))]
    public Book Book { get; set; }
}