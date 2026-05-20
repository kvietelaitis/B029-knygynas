using Microsoft.EntityFrameworkCore;
using Knygynas.Data;
using Knygynas.Models;

namespace Knygynas.Services;

public class BookstoreService : IBookstoreService
{
    private readonly ApplicationDbContext _context;

    public BookstoreService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bookstore>> GetAllBookstoresAsync(string? search)
    {
        var query = _context.Bookstores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var like = $"%{search.Trim()}%";
            query = query.Where(bookstore =>
                EF.Functions.Like(bookstore.City, like) ||
                EF.Functions.Like(bookstore.Address, like));
        }

        return await query
            .OrderBy(bookstore => bookstore.City)
            .ThenBy(bookstore => bookstore.Address)
            .ToListAsync();
    }

    public async Task<Bookstore?> GetBookstoreByIdAsync(int id)
    {
        return await _context.Bookstores.FindAsync(id);
    }

    public async Task<Bookstore> CreateBookstoreAsync(string city, string address)
    {
        var bookstore = new Bookstore
        {
            City = city,
            Address = address,
            CreatedDate = DateTime.UtcNow
        };

        _context.Add(bookstore);
        await _context.SaveChangesAsync();
        return bookstore;
    }

    public async Task<bool> UpdateBookstoreAsync(int id, string city, string address)
    {
        var bookstore = await _context.Bookstores.FindAsync(id);
        if (bookstore == null)
        {
            return false;
        }

        bookstore.City = city;
        bookstore.Address = address;

        try
        {
            _context.Update(bookstore);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookstoreExists(id))
            {
                return false;
            }
            throw;
        }
    }

    public async Task<bool> DeleteBookstoreAsync(int id)
    {
        var bookstore = await _context.Bookstores.FindAsync(id);
        if (bookstore == null)
        {
            return false;
        }

        _context.Bookstores.Remove(bookstore);
        await _context.SaveChangesAsync();
        return true;
    }

    public bool BookstoreExists(int id)
    {
        return _context.Bookstores.Any(e => e.Id == id);
    }
}
