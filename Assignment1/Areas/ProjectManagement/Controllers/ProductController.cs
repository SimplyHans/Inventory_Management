using Microsoft.AspNetCore.Mvc;
using Assignment1.Areas.ProjectManagement.Models;
using Assignment1.Data; // Add this namespace for ApplicationDbContext
using Microsoft.EntityFrameworkCore; // Add this for EF Core operations
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Areas.ProjectManagement.Controllers;

[Area("ProjectManagement")]
[Route("[area]/[controller]/[action]")]

    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Inject ApplicationDbContext via constructor
        public ProductController(ApplicationDbContext context, ILogger<ProductController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index(string searchQuery, string category, string sortBy, bool lowStockFilter = false)
        {
            try
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
            
                _logger.LogInformation("Accessed ProductController Index at {Time}", DateTime.Now);
                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products at {Time}", DateTime.Now);
                TempData["ErrorMessage"] = "An error occurred while fetching the products. Please try again later.";
                return View();
            }
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Fetch categories for the dropdown
                var categories = await _context.Categories.ToListAsync();
                ViewBag.Categories = categories;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories for product creation.");
                TempData["ErrorMessage"] = "An error occurred while loading the page. Please try again later.";
                return View();

            }
        }

        private async Task<bool> IsUserAdmin()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return false;
            }

            // Check the IsAdmin property of the user
            return user.IsAdmin;
        }

        
        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Add anti-forgery token for security
        public async Task<IActionResult> Create(Product product)
        {
            if (!await IsUserAdmin())
            {
                _logger.LogWarning("Permission to create product was denied.");
                TempData["ErrorMessage"] = "You do not have permission to create a product.";
                return RedirectToAction(nameof(Index));
            }
            
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
            try
            {
                // Retrieves the product with the specified ID or returns null if not found
                var product = _context.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return NotFound(); // 404 not found error
                }
                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking details at {Time}", DateTime.Now);
                TempData["ErrorMessage"] = "An error occurred while checking details. Please try again later.";
                return View();
            }
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

            try
            {
                // Fetch categories for the dropdown
                var categories = await _context.Categories.ToListAsync();
                ViewBag.Categories = categories;

                return View(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing at {Time}", DateTime.Now);
                TempData["ErrorMessage"] = "An error occurred while editing. Please try again later.";
                return View();
            }
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Add anti-forgery token for security
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (!await IsUserAdmin())
            {
                _logger.LogWarning("Permission to edit product was denied.");
                TempData["ErrorMessage"] = "You do not have permission to edit a product.";
                return RedirectToAction(nameof(Index));
            }
            
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
            if (!await IsUserAdmin())
            {
                _logger.LogWarning("Permission to delete product was denied.");
                TempData["ErrorMessage"] = "You do not have delete to create a product.";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
