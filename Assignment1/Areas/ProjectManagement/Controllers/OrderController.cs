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



    public IActionResult Cart()
    {
        try
        {
            _logger.LogInformation("Accessing Cart at {Time}", DateTime.Now);
            var cartItems = _context.CartItems.ToList();
            return View(cartItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accessing Cart at {Time}", DateTime.Now);
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        try
        {
            _logger.LogInformation("Attempting to add product {ProductId} to cart with quantity {Quantity} at {Time}", productId, quantity, DateTime.Now);
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null || quantity <= 0 || quantity > product.Quantity)
            {
                _logger.LogWarning("Invalid product or quantity for product {ProductId} at {Time}", productId, DateTime.Now);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding product {ProductId} to cart at {Time}", productId, DateTime.Now);
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int id)
    {
        try
        {
            _logger.LogInformation("Attempting to remove item {Id} from cart at {Time}", id, DateTime.Now);
            var cartItem = _context.CartItems.FirstOrDefault(c => c.Id == id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
                _logger.LogInformation("Item {Id} removed from cart successfully at {Time}", id, DateTime.Now);
            }
            return RedirectToAction(nameof(Cart));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while removing item {Id} from cart at {Time}", id, DateTime.Now);
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        try
        {
            _logger.LogInformation("Attempting to place order at {Time}", DateTime.Now);
            var cartItems = _context.CartItems.ToList();

            if (!cartItems.Any())
            {
                _logger.LogWarning("Attempted to place empty order at {Time}", DateTime.Now);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while placing order at {Time}", DateTime.Now);
            return View("Error");
        }
    }

    public IActionResult Track()
    {
        try
        {
            _logger.LogInformation("Accessing Order Track at {Time}", DateTime.Now);
            var orders = _context.Orders.ToList();
            return View("Track", orders); // ✅ Make sure Track.cshtml exists
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accessing Order Track at {Time}", DateTime.Now);
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult CancelOrder(int id)
    {
        try
        {
            _logger.LogInformation("Attempting to cancel order {Id} at {Time}", id, DateTime.Now);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cancelling order {Id} at {Time}", id, DateTime.Now);
            return View("Error");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}

