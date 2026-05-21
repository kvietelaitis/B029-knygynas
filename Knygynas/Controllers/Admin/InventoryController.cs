using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Knygynas.Services.Book;

namespace Knygynas.Controllers.Admin;

[Authorize(Roles = "Admin,Worker")]
public class InventoryController : Controller
{
    private readonly BookService _bookService;

    public InventoryController(BookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        return View(books);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Worker")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(string isbn, int quantity)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return BadRequest(new { success = false, message = "ISBN required" });
        if (quantity < 0) return BadRequest(new { success = false, message = "Quantity must be >= 0" });

        var ok = await _bookService.SetStockAsync(isbn, quantity);
        if (!ok) return NotFound(new { success = false, message = "Book not found or update failed" });

        return Json(new { success = true, isbn, quantity });
    }
}

