using Microsoft.AspNetCore.Mvc;
using Assignment1.Data; // Ensure this matches your project's namespace
using Assignment1.Models; // Ensure this matches your project's namespace
using Microsoft.EntityFrameworkCore; // Add this for EF Core operations
using System.Threading.Tasks;
using Assignment1.Areas.ProjectManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Assignment1.Areas.ProjectManagement.Controllers; 

[Area("ProjectManagement")]
[Route("[area]/[controller]/[action]")]

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    // Inject ApplicationDbContext and ILogger via constructor
    public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    // GET: Category/Index
    public async Task<IActionResult> Index()
    {
        try
        {
            _logger.LogInformation("Accessing Category Index at {Time}", DateTime.Now);
            // Fetch categories from the database asynchronously
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accessing Category Index at {Time}", DateTime.Now);
            return View("Error");
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
    
    

    // GET: Category/Create
    public IActionResult Create()
    {
        try
        {
            _logger.LogInformation("Accessing Category Create view at {Time}", DateTime.Now);
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accessing Category Create view at {Time}", DateTime.Now);
            return View("Error");
        }
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken] // Add this for security to prevent CSRF attacks
    public async Task<IActionResult> Create(Category category)
    {
        try
        {
            _logger.LogInformation("Attempting to create category at {Time}", DateTime.Now);
            if (!await IsUserAdmin())
            {
                _logger.LogWarning("Permission to create category was denied.");
                TempData["ErrorMessage"] = "You do not have permission to category a product.";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                // Add the category to the database asynchronously
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Category created successfully at {Time}", DateTime.Now);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category at {Time}", DateTime.Now);
            return View("Error");
        }
    }

    // GET: Category/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            _logger.LogInformation("Accessing Category Edit view for ID {Id} at {Time}", id, DateTime.Now);
            // Find the category by ID asynchronously
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found at {Time}", id, DateTime.Now);
                return NotFound();
            }
            return View(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accessing Category Edit view for ID {Id} at {Time}", id, DateTime.Now);
            return View("Error");
        }
    }

    // POST: Category/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken] // Add this for security to prevent CSRF attacks
    public async Task<IActionResult> Edit(int id, Category category)
    {
        try
        {
            _logger.LogInformation("Attempting to edit category with ID {Id} at {Time}", id, DateTime.Now);
            if (id != category.CategoryId)
            {
                _logger.LogWarning("Category ID mismatch at {Time}", DateTime.Now);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the category in the database asynchronously
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Category with ID {Id} updated successfully at {Time}", id, DateTime.Now);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!_context.Categories.Any(c => c.CategoryId == category.CategoryId))
                    {
                        _logger.LogWarning("Category with ID {Id} not found during update at {Time}", id, DateTime.Now);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error while updating category with ID {Id} at {Time}", id, DateTime.Now);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while editing category with ID {Id} at {Time}", id, DateTime.Now);
            return View("Error");
        }
    }

    // GET: Category/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation("Accessing Category Delete view for ID {Id} at {Time}", id, DateTime.Now);
            // Find the category by ID asynchronously
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found at {Time}", id, DateTime.Now);
                return NotFound();
            }
            return View(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while accessing Category Delete view for ID {Id} at {Time}", id, DateTime.Now);
            return View("Error");
        }
    }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken] // Add this for security to prevent CSRF attacks
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            _logger.LogInformation("Attempting to delete category with ID {Id} at {Time}", id, DateTime.Now);
            // Find the category by ID and remove it asynchronously
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Category with ID {Id} deleted successfully at {Time}", id, DateTime.Now);
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category with ID {Id} at {Time}", id, DateTime.Now);
            return View("Error");
        }
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.CategoryId == id);
    }
}
