using Microsoft.AspNetCore.Mvc;
using Assignment1.Models;
using Assignment1.Data; // Add this namespace for ApplicationDbContext
using Microsoft.EntityFrameworkCore; // Add this for EF Core operations
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext via constructor
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index(string searchQuery, string category, string sortBy, bool lowStockFilter = false)
        {
            // Start with all products
            var productsQuery = _context.Products.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchQuery));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category == category);
            }

            // Apply low stock filter
            if (lowStockFilter)
            {
                productsQuery = productsQuery.Where(p => p.Quantity < p.LowStockThreshold);
            }

            // Apply sorting
            switch (sortBy)
            {
                case "name_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Name);
                    break;
                case "price_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Name); // Default sorting
                    break;
            }

            // Fetch categories for the dropdown
            var categories = await _context.Categories.ToListAsync();

            // Pass categories to the view using ViewBag
            ViewBag.Categories = categories;

            // Execute the query and pass products to the view
            var products = await productsQuery.ToListAsync();
            return View(products);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            // Fetch categories for the dropdown
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Add anti-forgery token for security
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                // Add the product to the database
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, re-fetch categories and return to the view
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            TempData["ErrorMessage"] = "Failed to create product. Please check your input.";
            return View(product);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Retrieves the product with the specified ID or returns null if not found
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // 404 not found error
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Find the product by ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Fetch categories for the dropdown
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;

            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Add anti-forgery token for security
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the product in the database
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Product updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(p => p.Id == product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, re-fetch categories and return to the view
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            TempData["ErrorMessage"] = "Failed to update product. Please check your input.";
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Find the product by ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Add anti-forgery token for security
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the product by ID and remove it
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}