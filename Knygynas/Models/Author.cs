namespace Knygynas.Models;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Biography { get; set; }
    
    public ICollection<AuthorRole> AuthorRoles { get; set; }
    
    public Author()
    {
        AuthorRoles = new List<AuthorRole>();
    }
}