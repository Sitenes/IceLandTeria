using System.Globalization;
using IceLandTeria.Data;
using IceLandTeria.Models;
using IceLandTeria.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IceLandTeria.Controllers;

public class ProductsController : Controller
{
    private string GetRoute(string action) => $"~/Views/Admin/Products/{action}.cshtml";
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ApplicationDbContext context,
        ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        var productQuery = _context.Products
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            productQuery = productQuery.Where(product =>
                product.Title.Contains(search));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            productQuery = productQuery.Where(product =>
                product.CategoryId == categoryId.Value);
        }

        var products = await productQuery
            .OrderBy(product => product.SortOrder)
            .ThenBy(product => product.Id)
            .Select(product => new ProductListItemViewModel
            {
                Id = product.Id,
                Title = product.Title,
                CategoryTitle = product.Category != null
                    ? product.Category.Title
                    : "بدون دسته‌بندی",
                Price = product.Price,
                Description = product.Description,
                SortOrder = product.SortOrder,
                UpdatedAt = product.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Id)
            .Select(category => new CategoryOptionViewModel
            {
                Id = category.Id,
                Title = category.Title
            })
            .ToListAsync(cancellationToken);

        var viewModel = new ProductListViewModel
        {
            Products = products,
            Categories = categories,
            Search = search,
            CategoryId = categoryId
        };

        return View(GetRoute(nameof(Index)), viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create(
        CancellationToken cancellationToken)
    {
        var categories = await GetCategoryOptionsAsync(cancellationToken);

        if (categories.Count == 0)
        {
            TempData["ErrorMessage"] =
                "برای افزودن محصول، ابتدا حداقل یک دسته‌بندی ایجاد کنید.";

            return Redirect("/Admin/Products");
        }

        var viewModel = new CreateProductViewModel
        {
            Categories = categories
        };

        return View(GetRoute(nameof(Create)), viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateProductViewModel model,
        CancellationToken cancellationToken)
    {
        model.Title = model.Title?.Trim() ?? string.Empty;
        model.Description = string.IsNullOrWhiteSpace(model.Description)
            ? null
            : model.Description.Trim();

        if (!TryParsePrice(model.Price, out var price))
        {
            ModelState.AddModelError(
                nameof(model.Price),
                "قیمت واردشده معتبر نیست.");
        }
        else if (price < 0)
        {
            ModelState.AddModelError(
                nameof(model.Price),
                "قیمت نمی‌تواند منفی باشد.");
        }

        if (model.CategoryId <= 0)
        {
            ModelState.AddModelError(
                nameof(model.CategoryId),
                "لطفاً یک دسته‌بندی انتخاب کنید.");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoryOptionsAsync(cancellationToken);
            return View(GetRoute(nameof(Create)), model);
        }

        var categoryExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(
                category => category.Id == model.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            ModelState.AddModelError(
                nameof(model.CategoryId),
                "دسته‌بندی انتخاب‌شده وجود ندارد.");

            model.Categories = await GetCategoryOptionsAsync(cancellationToken);
            return View(GetRoute(nameof(Create)), model);
        }

        var now = DateTime.UtcNow;

        var product = new Product
        {
            CategoryId = model.CategoryId,
            Title = model.Title,
            Price = price,
            Description = model.Description,
            SortOrder = model.SortOrder,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            _context.Products.Add(product);

            await _context.SaveChangesAsync(cancellationToken);

            TempData["SuccessMessage"] =
                "محصول با موفقیت اضافه شد.";

            return Redirect("/Admin/Products");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Error while creating product.");

            ModelState.AddModelError(
                string.Empty,
                "ذخیره محصول انجام نشد. لطفاً دوباره تلاش کنید.");

            model.Categories = await GetCategoryOptionsAsync(cancellationToken);

            return View(GetRoute(nameof(Create)), model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var product = await _context.Products
            .AsNoTracking()
            .Where(product => product.Id == id)
            .Select(product => new
            {
                product.Id,
                product.CategoryId,
                product.Title,
                product.Price,
                product.Description,
                product.SortOrder
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        var categories = await GetCategoryOptionsAsync(cancellationToken);

        var viewModel = new EditProductViewModel
        {
            Id = product.Id,
            CategoryId = product.CategoryId,
            Title = product.Title,
            Price = product.Price.ToString(
                "0.##",
                CultureInfo.InvariantCulture),
            Description = product.Description,
            SortOrder = product.SortOrder,
            Categories = categories
        };

        return View(GetRoute(nameof(Edit)), viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EditProductViewModel model,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        if (id != model.Id)
        {
            return BadRequest();
        }

        model.Title = model.Title?.Trim() ?? string.Empty;
        model.Description = string.IsNullOrWhiteSpace(model.Description)
            ? null
            : model.Description.Trim();

        if (!TryParsePrice(model.Price, out var price))
        {
            ModelState.AddModelError(
                nameof(model.Price),
                "قیمت واردشده معتبر نیست.");
        }
        else if (price < 0)
        {
            ModelState.AddModelError(
                nameof(model.Price),
                "قیمت نمی‌تواند منفی باشد.");
        }

        if (model.CategoryId <= 0)
        {
            ModelState.AddModelError(
                nameof(model.CategoryId),
                "لطفاً یک دسته‌بندی انتخاب کنید.");
        }

        if (!ModelState.IsValid)
        {
            model.Categories = await GetCategoryOptionsAsync(cancellationToken);
            return View(GetRoute(nameof(Edit)), model);
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        var categoryExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(
                category => category.Id == model.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            ModelState.AddModelError(
                nameof(model.CategoryId),
                "دسته‌بندی انتخاب‌شده وجود ندارد.");

            model.Categories = await GetCategoryOptionsAsync(cancellationToken);

            return View(GetRoute(nameof(Edit)), model);
        }

        product.CategoryId = model.CategoryId;
        product.Title = model.Title;
        product.Price = price;
        product.Description = model.Description;
        product.SortOrder = model.SortOrder;
        product.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            TempData["SuccessMessage"] =
                "محصول با موفقیت ویرایش شد.";

            return Redirect("/Admin/Products");
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Error while updating product {ProductId}.",
                id);

            ModelState.AddModelError(
                string.Empty,
                "ویرایش محصول انجام نشد. لطفاً دوباره تلاش کنید.");

            model.Categories = await GetCategoryOptionsAsync(cancellationToken);

            return View(GetRoute(nameof(Edit)), model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);

        if (product is null)
        {
            TempData["ErrorMessage"] =
                "محصول موردنظر پیدا نشد.";

            return Redirect("/Admin/Products");
        }

        try
        {
            _context.Products.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            TempData["SuccessMessage"] =
                "محصول با موفقیت حذف شد.";
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(
                exception,
                "Error while deleting product {ProductId}.",
                id);

            TempData["ErrorMessage"] =
                "حذف محصول انجام نشد. لطفاً دوباره تلاش کنید.";
        }

        return Redirect("/Admin/Products");
    }

    private async Task<List<CategoryOptionViewModel>> GetCategoryOptionsAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Id)
            .Select(category => new CategoryOptionViewModel
            {
                Id = category.Id,
                Title = category.Title
            })
            .ToListAsync(cancellationToken);
    }

    private static bool TryParsePrice(
        string? value,
        out decimal price)
    {
        price = 0;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = NormalizeNumber(value);

        return decimal.TryParse(
            normalized,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out price);
    }

    private static string NormalizeNumber(string value)
    {
        return value
            .Trim()
            .Replace("٬", string.Empty)
            .Replace(",", string.Empty)
            .Replace("،", string.Empty)
            .Replace(" ", string.Empty)
            .Replace("٫", ".")
            .Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9");
    }
}