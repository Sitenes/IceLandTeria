using IceLandTeria.Data;
using IceLandTeria.Models;
using IceLandTeria.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceLandTeria.Controllers;

public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;
    private string GetRoute(string action) => $"~/Views/Admin/Categories/{action}.cshtml";
    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Select(category => new CategoryListViewModel
            {
                Id = category.Id,
                Title = category.Title,
                SortOrder = category.SortOrder,
                ProductCount = _context.Products.Count(
                    product => product.CategoryId == category.Id)
            })
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Id)
            .ToListAsync();

        return View(GetRoute(nameof(Index)), categories);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(GetRoute(nameof(Create)), new CategoryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var category = new Category
        {
            Title = model.Title.Trim(),
            SortOrder = model.SortOrder
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "دسته‌بندی با موفقیت ایجاد شد.";

        return Redirect("/Admin/Categories");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        var model = new CategoryFormViewModel
        {
            Title = category.Title,
            SortOrder = category.SortOrder
        };

        ViewData["CategoryId"] = category.Id;

        return View(GetRoute(nameof(Edit)), model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CategoryFormViewModel model)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["CategoryId"] = id;

            return View(GetRoute(nameof(Edit)), model);
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        category.Title = model.Title.Trim();
        category.SortOrder = model.SortOrder;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            "دسته‌بندی با موفقیت ویرایش شد.";

        return Redirect("/Admin/Categories");
    }

    [HttpPost()]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(category => category.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        var hasProducts = await _context.Products
            .AnyAsync(product => product.CategoryId == id);

        if (hasProducts)
        {
            TempData["ErrorMessage"] =
                "این دسته‌بندی دارای محصول است و تا زمانی که محصولات آن حذف یا منتقل نشوند، قابل حذف نیست.";

            return Redirect("/Admin/Categories");
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"دسته‌بندی «{category.Title}» با موفقیت حذف شد.";

        return Redirect("/Admin/Categories");
    }
}