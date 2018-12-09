using CoreApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreApp.Application.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? PromotionPrice { get; set; }

        [Required]
        public decimal OriginalPrice { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string Content { get; set; }

        public bool HotFlag { get; set; }

        public int? ViewCount { get; set; }

        public string Tags { get; set; }

        [StringLength(50)]
        public string Unit { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        [StringLength(250)]
        public string SeoPageTitle { get; set; }

        [StringLength(250)]
        public string SeoAlias { get; set; }

        [StringLength(250)]
        public string SeoKeywords { get; set; }

        [StringLength(250)]
        public string SeoDescription { get; set; }

        public Status Status { get; set; }

        public ProductCategoryViewModel ProductCategory { get; set; }

        public virtual ICollection<ProductTagViewModel> ProductTags { get; } = new List<ProductTagViewModel>();
        public virtual ICollection<ProductQuantityViewModel> ProductQuantities { get; set; }
        public virtual ICollection<ProductImageViewModel> ProductImages { get; set; }
        public virtual ICollection<BillDetailViewModel> BillDetails { get; set; }
        public virtual ICollection<WholePriceViewModel> WholePrices { get; set; }
    }
}
