using Microsoft.AspNetCore.Mvc;
using Assignment1.Data;
using Assignment1.Models;

namespace Assignment1.Controllers
{
    public class OrderListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderListController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToOrderList(int productId, int quantity)
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
    }
}