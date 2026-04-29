namespace Knygynas.Services.Book;

public class BookFilter
{
    public string? Title { get; set; }
    public string? AuthorName { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
}