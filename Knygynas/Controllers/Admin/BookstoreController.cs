using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Knygynas.Models;
using Knygynas.Services;

namespace Knygynas.Controllers.Admin;

public class BookstoreController : Controller
{
    private readonly IBookstoreService _bookstoreService;

    public BookstoreController(IBookstoreService bookstoreService)
    {
        _bookstoreService = bookstoreService;
    }

    // GET: Admin/Bookstore
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var bookstores = await _bookstoreService.GetAllBookstoresAsync();
        return View(bookstores);
    }

    // GET: Admin/Bookstore/Details/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookstore = await _bookstoreService.GetBookstoreByIdAsync(id.Value);
        if (bookstore == null)
        {
            return NotFound();
        }

        return View(bookstore);
    }

    // GET: Admin/Bookstore/Create
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Bookstore/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([Bind("City,Address")] Bookstore bookstore)
    {
        if (ModelState.IsValid)
        {
            await _bookstoreService.CreateBookstoreAsync(bookstore.City, bookstore.Address);
            return RedirectToAction(nameof(Index));
        }
        return View(bookstore);
    }

    // GET: Admin/Bookstore/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookstore = await _bookstoreService.GetBookstoreByIdAsync(id.Value);
        if (bookstore == null)
        {
            return NotFound();
        }
        return View(bookstore);
    }

    // POST: Admin/Bookstore/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, [Bind("Id,City,Address,CreatedDate")] Bookstore bookstore)
    {
        if (id != bookstore.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var success = await _bookstoreService.UpdateBookstoreAsync(id, bookstore.City, bookstore.Address);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
        return View(bookstore);
    }

    // GET: Admin/Bookstore/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookstore = await _bookstoreService.GetBookstoreByIdAsync(id.Value);
        if (bookstore == null)
        {
            return NotFound();
        }

        return View(bookstore);
    }

    // POST: Admin/Bookstore/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _bookstoreService.DeleteBookstoreAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
