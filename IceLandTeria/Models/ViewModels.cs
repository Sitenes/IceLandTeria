using System.ComponentModel.DataAnnotations;

namespace IceLandTeria.ViewModels;

public class CategoryFormViewModel
{
    [Required(
        ErrorMessage = "عنوان دسته‌بندی الزامی است.")]
    [StringLength(
        100,
        ErrorMessage = "عنوان دسته‌بندی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
    [Display(Name = "عنوان دسته‌بندی")]
    public string Title { get; set; } = string.Empty;

    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "ترتیب نمایش باید یک عدد صحیح بزرگ‌تر یا مساوی صفر باشد.")]
    [Display(Name = "ترتیب نمایش")]
    public int SortOrder { get; set; }
}
public class CategoryListViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int SortOrder { get; init; }

    public int ProductCount { get; init; }
}

public class EditProductViewModel
{
    public int Id { get; set; }

    [Display(Name = "دسته‌بندی")]
    [Range(1, int.MaxValue, ErrorMessage = "لطفاً یک دسته‌بندی انتخاب کنید.")]
    public int CategoryId { get; set; }

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "عنوان محصول الزامی است.")]
    [StringLength(200, ErrorMessage = "عنوان محصول نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "قیمت")]
    [Required(ErrorMessage = "قیمت محصول الزامی است.")]
    public string Price { get; set; } = string.Empty;

    [Display(Name = "توضیحات")]
    [StringLength(2000, ErrorMessage = "توضیحات نمی‌تواند بیشتر از ۲۰۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }

    [Display(Name = "ترتیب نمایش")]
    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش باید یک عدد صحیح صفر یا بیشتر باشد.")]
    public int SortOrder { get; set; }

    public IReadOnlyList<CategoryOptionViewModel> Categories { get; set; }
        = Array.Empty<CategoryOptionViewModel>();
}

public class ProductListViewModel
{
    public IReadOnlyList<ProductListItemViewModel> Products { get; set; }
        = Array.Empty<ProductListItemViewModel>();

    public IReadOnlyList<CategoryOptionViewModel> Categories { get; set; }
        = Array.Empty<CategoryOptionViewModel>();

    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public bool HasFilters =>
        !string.IsNullOrWhiteSpace(Search) || CategoryId.HasValue;
}

public class ProductListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string CategoryTitle { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class CategoryOptionViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
}

public class CreateProductViewModel
{
    [Display(Name = "دسته‌بندی")]
    [Range(1, int.MaxValue, ErrorMessage = "لطفاً یک دسته‌بندی انتخاب کنید.")]
    public int CategoryId { get; set; }

    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "عنوان محصول الزامی است.")]
    [StringLength(200, ErrorMessage = "عنوان محصول نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "قیمت")]
    [Required(ErrorMessage = "قیمت محصول الزامی است.")]
    public string Price { get; set; } = string.Empty;

    [Display(Name = "توضیحات")]
    [StringLength(2000, ErrorMessage = "توضیحات نمی‌تواند بیشتر از ۲۰۰۰ کاراکتر باشد.")]
    public string? Description { get; set; }

    [Display(Name = "ترتیب نمایش")]
    [Range(0, int.MaxValue, ErrorMessage = "ترتیب نمایش باید یک عدد صحیح صفر یا بیشتر باشد.")]
    public int SortOrder { get; set; }

    public IReadOnlyList<CategoryOptionViewModel> Categories { get; set; }
        = Array.Empty<CategoryOptionViewModel>();
}

public class MenuViewModel
{
    public List<MenuCategoryViewModel> Categories { get; set; } = new();
}

public class MenuCategoryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<MenuProductViewModel> Products { get; set; } = new();
}

public class MenuProductViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
}