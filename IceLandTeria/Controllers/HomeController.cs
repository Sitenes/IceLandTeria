using IceLandTeria.Data;
using IceLandTeria.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(category => new MenuCategoryViewModel
            {
                Id = category.Id,
                Title = category.Title,

                Products = _context.Products
                    .AsNoTracking()
                    .Where(product => product.CategoryId == category.Id)
                    .OrderBy(product => product.SortOrder)
                    .ThenBy(product => product.Id)
                    .Select(product => new MenuProductViewModel
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Description = product.Description,
                        Price = product.Price
                    })
                    .ToList()
            })
            .ToListAsync();

        var model = new MenuViewModel
        {
            Categories = categories
        };

        return View(model);
    }
}