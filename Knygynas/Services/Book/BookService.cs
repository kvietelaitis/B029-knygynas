using Knygynas.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Knygynas.Models;
using System.Threading.Tasks;

namespace Knygynas.Services.Book;

public class BookService
{
    private readonly ApplicationDbContext _context;
    
    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Synchronous convenience method (kept for callers that use it)
    public Models.Book GetBook(string id)
    {
        return GetBookAsync(id).GetAwaiter().GetResult();
    }

    // Async variant used by controllers/services to better separate responsibilities
    public async Task<Models.Book?> GetBookAsync(string id)
    {
        var book = await _context.Books
            .Include(b => b.AuthorRoles).ThenInclude(ar => ar.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Where(b => b.ISBN.Equals(id))
            .FirstOrDefaultAsync();

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

    public async Task<List<Models.Book>> GetAllBooksAsync()
    {
        return await _context.Books
            .Include(b => b.AuthorRoles).ThenInclude(ar => ar.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .ToListAsync();
    }

    // Decrement stock in an atomic, savepoint-aware fashion
    public async Task<bool> DecrementStockAsync(string isbn, int amount)
    {
        if (amount <= 0) return false;

        var book = await _context.Books.FindAsync(isbn);
        if (book == null) return false;

        if (book.Quantity < amount) return false;

        book.Quantity -= amount;
        try
        {
            _context.Update(book);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // reload and check
            var fresh = await _context.Books.FindAsync(isbn);
            if (fresh == null || fresh.Quantity < amount) return false;
            throw;
        }
    }

    public async Task<bool> SetStockAsync(string isbn, int quantity)
    {
        if (quantity < 0) return false;
        var book = await _context.Books.FindAsync(isbn);
        if (book == null) return false;

        book.Quantity = quantity;
        _context.Update(book);
        await _context.SaveChangesAsync();
        return true;
    }

    // Decrement multiple books in a single transaction/save operation.
    // Returns true if all decrements applied successfully; otherwise false and no changes are committed.
    public async Task<bool> DecrementMultipleAsync(IEnumerable<(string Isbn, int Amount)> items)
    {
        if (items == null) return false;

        // simple validation
        foreach (var it in items)
        {
            if (it.Amount <= 0) return false;
        }

        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check availability first
            foreach (var it in items)
            {
                var book = await _context.Books.FindAsync(it.Isbn);
                if (book == null || book.Quantity < it.Amount)
                {
                    await tx.RollbackAsync();
                    return false;
                }
                book.Quantity -= it.Amount;
                _context.Update(book);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return true;
        }
        catch
        {
            try { await tx.RollbackAsync(); } catch { }
            throw;
        }
    }
}