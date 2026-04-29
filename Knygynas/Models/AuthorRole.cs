namespace Knygynas.Models;

public class AuthorRole
{
    public string RoleName { get; set; }
    public int AuthorID { get; set; }
    public Author Author { get; set; }
    public string BookISBN { get; set; }
    public Book Book { get; set; }
}