using Microsoft.AspNetCore.Mvc;
using Assignment1.Data; // Ensure this matches your project's namespace
using Assignment1.Models; // Ensure this matches your project's namespace
using Microsoft.EntityFrameworkCore; // Add this for EF Core operations
using System.Threading.Tasks;
using Assignment1.Areas.ProjectManagement.Models;

namespace Assignment1.Areas.ProjectManagement.Controllers; 

[Area("ProjectManagement")]
[Route("[area]/[controller]/[action]")]

    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext via constructor
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Category/Index
        public async Task<IActionResult> Index()
        {
            // Fetch categories from the database asynchronously
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken] // Add this for security to prevent CSRF attacks
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // Add the category to the database asynchronously
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Find the category by ID asynchronously
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken] // Add this for security to prevent CSRF attacks
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the category in the database asynchronously
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(c => c.CategoryId == category.CategoryId))
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
            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Find the category by ID asynchronously
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Add this for security to prevent CSRF attacks
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the category by ID and remove it asynchronously
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
