using System.ComponentModel.DataAnnotations;

namespace formsApp.Models
{
    public class Product
    {
        [Display(Name = "Ürün Id")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Ürün Adı Gerekli!")]
        [Display(Name = "Ürün Adı")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Ürün Fiyatı Gerekli!")]
        [Display(Name = "Ürün Fiyatı")]
        public decimal? Price { get; set; }

        [Display(Name = "Ürün Resmi")]
        public string? Image { get; set; } = string.Empty;

        [Display(Name = "Ürün Aktif mi?")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Ürün Kategorisi Gerekli!")]
        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
    }
}