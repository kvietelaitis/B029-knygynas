using Microsoft.AspNetCore.Mvc;
using Knygynas.Models;
using Knygynas.Services.Book;
using Knygynas.Extensions;
using Knygynas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Knygynas.Controllers;

public class CartController : Controller
{
    private readonly BookService _bookService;
    private readonly ApplicationDbContext _context;
    private readonly Knygynas.Services.EmailService _emailService;
    private const string CartSessionKey = "ShoppingCart";

    public CartController(BookService bookService, ApplicationDbContext context, Knygynas.Services.EmailService emailService)
    {
        _bookService = bookService;
        _context = context;
        _emailService = emailService;
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
        if (cart.Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        // Verify all books in the cart are available
        foreach (var item in cart.Items)
        {
            var dbBook = _bookService.GetBook(item.ISBN);
            if (dbBook == null || !dbBook.Available)
            {
                TempData["ErrorMessage"] = $"The book '{item.Title}' is no longer available for purchase.";
                return RedirectToAction(nameof(Index));
            }
        }

        ViewBag.Postomats = await _context.Postomats.ToListAsync();
        ViewBag.Bookstores = await _context.Bookstores.ToListAsync();

        return View(cart);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Checkout(string deliveryMethod, int? selectedPostomatId, int? selectedBookstoreId, string? customCity, string? customAddress, string phoneNumber)
    {
        var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
        if (cart.Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            TempData["ErrorMessage"] = "Please provide your phone number for delivery confirmation.";
            return RedirectToAction(nameof(Checkout));
        }

        // Verify all books in the cart are available and in stock
        foreach (var item in cart.Items)
        {
            var dbBook = _bookService.GetBook(item.ISBN);
            if (dbBook == null || dbBook.Quantity < item.Quantity)
            {
                TempData["ErrorMessage"] = $"The book '{item.Title}' is out of stock or does not have enough copies left.";
                return RedirectToAction(nameof(Index));
            }
        }

        string toCity = "";
        string toAddress = "";

        if (deliveryMethod == "Postomat")
        {
            if (!selectedPostomatId.HasValue)
            {
                TempData["ErrorMessage"] = "Please select a postomat locker.";
                return RedirectToAction(nameof(Checkout));
            }
            var postomat = await _context.Postomats.FindAsync(selectedPostomatId.Value);
            if (postomat == null)
            {
                TempData["ErrorMessage"] = "Selected postomat was not found.";
                return RedirectToAction(nameof(Checkout));
            }
            toCity = postomat.City;
            toAddress = postomat.Address + $" ({postomat.Company} Postomat)";
        }
        else if (deliveryMethod == "Bookstore")
        {
            if (!selectedBookstoreId.HasValue)
            {
                TempData["ErrorMessage"] = "Please select a bookstore for pick-up.";
                return RedirectToAction(nameof(Checkout));
            }
            var bookstore = await _context.Bookstores.FindAsync(selectedBookstoreId.Value);
            if (bookstore == null)
            {
                TempData["ErrorMessage"] = "Selected bookstore was not found.";
                return RedirectToAction(nameof(Checkout));
            }
            toCity = bookstore.City;
            toAddress = bookstore.Address + " (Bookstore Pickup)";
        }
        else if (deliveryMethod == "Address")
        {
            if (string.IsNullOrWhiteSpace(customCity) || string.IsNullOrWhiteSpace(customAddress))
            {
                TempData["ErrorMessage"] = "Please provide your delivery city and address.";
                return RedirectToAction(nameof(Checkout));
            }
            toCity = customCity.Trim();
            toAddress = customAddress.Trim();
        }
        else
        {
            TempData["ErrorMessage"] = "Invalid delivery method selected.";
            return RedirectToAction(nameof(Checkout));
        }

        // Determine origin/where it's coming from
        var bookstores = await _context.Bookstores.ToListAsync();
        var fromBookstore = bookstores.FirstOrDefault(b => b.City == "Kaunas") ?? bookstores.FirstOrDefault() ?? new Bookstore { City = "Kaunas HQ", Address = "Savanorių pr. 1" };
        
        string fromCity = fromBookstore.City;
        string fromAddress = fromBookstore.Address;

        var userEmail = User.Identity?.Name ?? "unknown@bookstore.com";

        // Create the order in PendingPayment state
        var order = new Order
        {
            OrderDate = DateTime.UtcNow,
            TotalPrice = cart.TotalPrice,
            DeliveryMethod = deliveryMethod,
            FromCity = fromCity,
            FromAddress = fromAddress,
            ToCity = toCity,
            ToAddress = toAddress,
            State = OrderState.PendingPayment,
            UserId = userEmail,
            PhoneNumber = phoneNumber
        };

        foreach (var item in cart.Items)
        {
            order.OrderItems.Add(new OrderItem
            {
                BookISBN = item.ISBN,
                Price = item.Price,
                Quantity = item.Quantity,
                Book = _bookService.GetBook(item.ISBN)
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Integrate Stripe Checkout session
        try
        {
            var lineItems = new List<Stripe.Checkout.SessionLineItemOptions>();
            foreach (var item in cart.Items)
            {
                lineItems.Add(new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = (decimal)(item.Price * 100), // Stripe expects cents
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title,
                            Description = $"ISBN: {item.ISBN}"
                        }
                    },
                    Quantity = item.Quantity
                });
            }

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Cart/PaymentSuccess?orderId={order.Id}&sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Cart/PaymentCancelled?orderId={order.Id}",
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", order.Id.ToString() }
                }
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = await service.CreateAsync(options);

            // Save Stripe Session ID
            order.StripeSessionId = session.Id;
            await _context.SaveChangesAsync();

            // Redirect user directly to Stripe Hosted Checkout
            return Redirect(session.Url);
        }
        catch (Exception ex)
        {
            // Rollback order creation on failure
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["ErrorMessage"] = $"Stripe checkout initialisation failed: {ex.Message}";
            return RedirectToAction(nameof(Checkout));
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> PaymentSuccess(int orderId, string sessionId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        if (order.State == OrderState.PendingPayment)
        {
            try
            {
                var service = new Stripe.Checkout.SessionService();
                var session = await service.GetAsync(sessionId);

                if (session != null && session.PaymentStatus == "paid")
                {
                    // Concurrency check: verify quantities before decrementing
                    foreach (var item in order.OrderItems)
                    {
                        var book = await _context.Books.FindAsync(item.BookISBN);
                        if (book == null || book.Quantity < item.Quantity)
                        {
                            order.State = OrderState.Cancelled;
                            await _context.SaveChangesAsync();

                            TempData["ErrorMessage"] = $"Unfortunately, '{item.Book?.Title ?? item.BookISBN}' went out of stock during your payment transaction. A refund has been issued.";
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    // Decrement stock levels
                    foreach (var item in order.OrderItems)
                    {
                        var book = await _context.Books.FindAsync(item.BookISBN);
                        book.Quantity -= item.Quantity;
                    }

                    // Complete order transition
                    order.State = OrderState.Processing;
                    await _context.SaveChangesAsync();

                    // Send order confirmation email
                    var userEmail = User.Identity?.Name ?? "unknown@bookstore.com";
                    await _emailService.SendOrderConfirmationEmail(userEmail, order);

                    // Clear shopping cart
                    var cart = HttpContext.Session.GetObject<Cart>(CartSessionKey) ?? new Cart();
                    cart.Clear();
                    HttpContext.Session.SetObject(CartSessionKey, cart);

                    TempData["SuccessMessage"] = $"Payment succeeded! Order #{order.Id} has been successfully placed.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Stripe payment verification failed.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Payment verification error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> PaymentCancelled(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null && order.State == OrderState.PendingPayment)
        {
            order.State = OrderState.Cancelled;
            await _context.SaveChangesAsync();
        }

        TempData["ErrorMessage"] = "Stripe Checkout session was cancelled. Your shopping cart remains intact.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> OrderSuccess(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> MyOrders()
    {
        var userEmail = User.Identity?.Name;
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .Where(o => o.UserId == userEmail)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }
}
