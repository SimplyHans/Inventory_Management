using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;
using Assignment1.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Assignment1.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult index(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null || quantity <= 0 || quantity > product.Quantity)
            {
                return NotFound(); // Handle invalid input
            }

            // Create a new OrderList entry
            var orderList = new OrderList
            {
                ProductId = productId,
                Quantity = quantity,
                OrderId = null // Assign to an order later
            };

            _context.OrderLists.Add(orderList);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home"); // Redirect back to the homepage
        }

        // POST: Cart/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Find the product by ID
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Check if the product is already in the cart
            var cartItem = await _context.Cart
                .Include(c => c.Product) // Include the Product navigation property
                .FirstOrDefaultAsync(c => c.ProductId == productId);

            if (cartItem != null)
            {
                // If the product is already in the cart, increase the quantity
                cartItem.Quantity++;
            }
            else
            {
                // If the product is not in the cart, add it
                cartItem = new Cart
                {
                    ProductId = product.Id, // Assign the ProductId
                    Quantity = 1 // Set the initial quantity to 1
                };
                _context.Cart.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Cart/Index
        public async Task<IActionResult> Index()
        {
            // Fetch cart items from the database, including the Product navigation property
            var cartItems = await _context.Cart
                .Include(c => c.Product) // Include the Product navigation property
                .ToListAsync();

            return View(cartItems);
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            // Find the cart item by ID
            var cartItem = await _context.Cart.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            // Remove the cart item from the database
            _context.Cart.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            // Validate the quantity
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            // Find the cart item by ID
            var cartItem = await _context.Cart.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }

            // Update the quantity
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Cart/ClearCart
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            // Fetch all cart items
            var cartItems = await _context.Cart.ToListAsync();

            // Remove all cart items from the database
            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}