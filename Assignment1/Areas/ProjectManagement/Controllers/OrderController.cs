using System.Diagnostics;
using Assignment1.Areas.ProjectManagement.Models;
using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Order = Assignment1.Areas.ProjectManagement.Models.Order;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace Assignment1.Areas.ProjectManagement.Controllers;

[Area("ProjectManagement")]
[Route("[area]/[controller]/[action]")]

public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(ILogger<OrderController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Cart()
    {
        var cartItems = _context.CartItems.ToList();
        return View(cartItems);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == productId);

        if (product == null || quantity <= 0 || quantity > product.Quantity)
        {
            TempData["ErrorMessage"] = "Invalid product or quantity.";
            return RedirectToAction("Index", "Home");
        }

        var cartItem = new CartItem
        {
            ProductName = product.Name,
            UnitPrice = product.Price,
            Quantity = quantity
        };

        _context.CartItems.Add(cartItem);
        _context.SaveChanges();

        TempData["Message"] = $"Added {quantity} x {product.Name} to your cart.";
        return RedirectToAction("Cart");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int id)
    {
        var item = _context.CartItems.FirstOrDefault(c => c.Id == id);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            _context.SaveChanges();
            TempData["Message"] = $"{item.ProductName} removed from cart.";
        }

        return RedirectToAction("Cart");
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        var cartItems = _context.CartItems.ToList();

        if (!cartItems.Any())
        {
            TempData["Message"] = "Cart is empty!";
            return RedirectToAction("Cart");
        }

        decimal total = 0;

        // Check stock and update quantities
        foreach (var item in cartItems)
        {
            var product = _context.Products.FirstOrDefault(p => p.Name == item.ProductName);
            if (product == null || product.Quantity < item.Quantity)
            {
                TempData["Message"] = $"Not enough stock for {item.ProductName}.";
                return RedirectToAction("Cart");
            }

            product.Quantity -= item.Quantity;
            total += item.Quantity * item.UnitPrice;
        }

        var user = await _userManager.GetUserAsync(User);
        var email = user?.Email ?? "guest@example.com";
        var customerName = user != null ? $"{user.FirstName} {user.LastName}" : "Guest";

        var order = new Order
        {
            CustomerName = customerName,
            Email = email,
            TotalAmount = total,
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
            _context.OrderItems.Add(orderItem);
        }

        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        TempData["Message"] = "Order placed successfully!";
        return RedirectToAction("Track");
    }

    public IActionResult Track()
    {
        var orders = _context.Orders.ToList();
        return View("Track", orders); // ✅ Make sure Track.cshtml exists
    }

    [HttpPost]
    public IActionResult CancelOrder(int id)
    {
        var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
        var orderItems = _context.OrderItems.Where(oi => oi.OrderId == id).ToList();

        if (order == null)
        {
            TempData["Message"] = "Order not found.";
            return RedirectToAction("Track");
        }

        // Restore inventory
        foreach (var item in orderItems)
        {
            var product = _context.Products.FirstOrDefault(p => p.Name == item.ProductName);
            if (product != null)
            {
                product.Quantity += item.Quantity;
            }
        }

        _context.OrderItems.RemoveRange(orderItems);
        _context.Orders.Remove(order);
        _context.SaveChanges();

        TempData["Message"] = $"Order #{id} cancelled and inventory restored.";
        return RedirectToAction("Track");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}

