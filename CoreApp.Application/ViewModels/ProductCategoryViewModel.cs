using CoreApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreApp.Application.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Image { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public int SortOrder { get; set; }

        [StringLength(250)]
        public string SeoPageTitle { get; set; }

        [StringLength(250)]
        public string SeoAlias { get; set; }

        [StringLength(250)]
        public string SeoKeywords { get; set; }

        [StringLength(250)]
        public string SeoDescription { get; set; }

        public Status Status { get; set; }

        public ICollection<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    }
}
