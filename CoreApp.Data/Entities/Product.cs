using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class Product : DomainEntity<int>, IDateTracking, IHasSeoMetaData, ISwitchable
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal? PromotionPrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public bool? HotFlag { get; set; }
        public int? ViewCount { get; set; }
        public string Tags { get; set; }
        public string Unit { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string SeoPageTitle { get; set; }
        public string SeoAlias { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public Status Status { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }

        public virtual ICollection<ProductTag> ProductTags { get; } = new List<ProductTag>();
        public virtual ICollection<ProductQuantity> ProductQuantities { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<WholePrice> WholePrices { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
