using Microsoft.AspNetCore.Mvc;
using Assignment1.Data; // Ensure this matches your project's namespace
using Assignment1.Models; // Ensure this matches your project's namespace
using Microsoft.EntityFrameworkCore; // Add this for EF Core operations
using System.Linq;
using System.Threading.Tasks;
using Assignment1.Areas.ProjectManagement.Models;

namespace Assignment1.Controllers // Ensure this matches your project's namespace
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        // Inject ApplicationDbContext via constructor
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index(string searchQuery, string category, string sortBy, bool lowStockFilter = false)
        {
            _logger.LogInformation("Accessed HomeController Index at {Time}", DateTime.Now);
            
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

            // Execute the query and create the view model
            var viewModel = new HomeIndexViewModel
            {
                Products = await productsQuery.ToListAsync(),
                Categories = categories
            };

            return View(viewModel);
        }

        // POST: Home/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category created successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to create category. Please check your input.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Home/DeleteCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Category not found.";
            }
            return RedirectToAction(nameof(Index));
        }
        
        
        
        public IActionResult NotFound(int statusCode)
        {
            _logger.LogWarning("Not Found invoked at {Time}", DateTime.Now);
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            return View("Error");
        }
    }
}