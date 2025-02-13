using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    public IActionResult Edit()
    {
        return View();
    }
    
    public IActionResult Delete()
    {
        return View();
    }
    
}