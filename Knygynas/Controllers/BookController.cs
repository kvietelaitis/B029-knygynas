using Knygynas.Data;
using Microsoft.AspNetCore.Mvc;
using Knygynas.Models;
using Knygynas.Services.Book;

namespace Knygynas.Controllers.Client;

public class BookController : Controller
{
    private readonly BookService _bookService;
    
    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }
    
    public IActionResult List([FromQuery] BookFilter filter)
    {
        var books = _bookService.GetBooks(filter);
        return View(books);
    }

    public IActionResult Details(string id)
    {
        var book = _bookService.GetBook(id);
        return View(book);
    }
}