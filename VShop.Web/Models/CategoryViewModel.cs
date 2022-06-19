using System.ComponentModel.DataAnnotations;

namespace VShop.Web.Models;
public class CategoryViewModel
{
    public int CategoryId { get; set; }
    [Required]
    public string? Name { get; set; }
}
