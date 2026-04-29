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
    
    public List<Models.Book> GetBooks()
    {
        var books = _context.Books.Include(b => b.AuthorRoles).ThenInclude(ar => ar.Author).ToList();

        return books;
    }
}