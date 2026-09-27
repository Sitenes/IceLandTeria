using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IceLandTeria.Models;

public class Category
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}