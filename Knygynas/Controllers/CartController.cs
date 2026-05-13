using Microsoft.AspNetCore.Mvc;
using Knygynas.Models;
using Knygynas.Services.Book;
using Knygynas.Extensions;

namespace Knygynas.Controllers;

public class CartController : Controller
{
    private readonly BookService _bookService;
    private const string CartSessionKey = "ShoppingCart";

    public CartController(BookService bookService)
    {
        _bookService = bookService;
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
        return View(cart);
    }

    [HttpPost]
    public IActionResult AddToCart(string isbn)
    {
        var book = _bookService.GetBook(isbn);
        if (book == null)
        {
            return NotFound();
        }

        var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
        cart.AddItem(book, 1);
        HttpContext.Session.SetObject(CartSessionKey, cart);

        return Json(new { success = true, title = book.Title, itemCount = cart.Items.Sum(i => i.Quantity) });
    }

    [HttpPost]
    public IActionResult RemoveFromCart(string isbn)
    {
        var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
        cart.RemoveItem(isbn);
        HttpContext.Session.SetObject(CartSessionKey, cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult UpdateQuantity(string isbn, int quantity)
    {
        var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
        cart.UpdateQuantity(isbn, quantity);
        HttpContext.Session.SetObject(CartSessionKey, cart);

        return RedirectToAction(nameof(Index));
    }
}
