using Knygynas.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Knygynas.Models;

namespace Knygynas.Services.Book;

public class BookService
{
    private readonly ApplicationDbContext _context;
    
    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Models.Book GetBook(string id)
    {
        var book = _context.Books
            .Include(b => b.AuthorRoles).ThenInclude(ar => ar.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Where(b => b.ISBN.Equals(id)).FirstOrDefault();
        
        return book;
    }
    
    public List<Models.Book> GetBooks(BookFilter filter)
    {
        var query = _context.Books
            .Include(b => b.AuthorRoles).ThenInclude(ar => ar.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
        {
            query = query.Where(b => b.Title.ToLower().Contains(filter.Title.ToLower()));
        }

        if (!string.IsNullOrEmpty(filter.AuthorName))
        {
            query = query.Where(b => b.AuthorRoles.Any(ar => ar.Author.Name.ToLower() == filter.AuthorName.ToLower()));
        }

        if (filter.YearFrom.HasValue)
        {
            query = query.Where(b => b.ReleaseDate.Year >= filter.YearFrom.Value);
        }

        if (filter.YearTo.HasValue)
        {
            query = query.Where(b => b.ReleaseDate.Year <= filter.YearTo.Value);
        }

        return query.ToList();
    }
}