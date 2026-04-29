using Knygynas.Models;

namespace Knygynas.Services;

public interface IBookstoreService
{
    Task<IEnumerable<Bookstore>> GetAllBookstoresAsync();
    Task<Bookstore?> GetBookstoreByIdAsync(int id);
    Task<Bookstore> CreateBookstoreAsync(string city, string address);
    Task<bool> UpdateBookstoreAsync(int id, string city, string address);
    Task<bool> DeleteBookstoreAsync(int id);
    bool BookstoreExists(int id);
}
