using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.DTOs;
public class ProductDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "The Name is Required")]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "The Description is Required")]
    [MinLength(5)]
    [MaxLength(200)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "The Price is Required")]
    [Column(TypeName = "decimal(12,2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "The Stock is Required")]
    [Range(1, 9999)]
    public long Stock { get; set; }

    [MaxLength(250)]
    [DisplayName("Product Image")]
    public string? ImageURL { get; set; }

    public string? CategoryName { get; set; }

    public int CategoryId { get; set; }
    [JsonIgnore]
    public Category? Category { get; set; }   
}
    