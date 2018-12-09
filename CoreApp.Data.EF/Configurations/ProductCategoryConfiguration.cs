using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ProductCategoryConfiguration : DbEntityConfiguration<ProductCategory>
    {
        public override void Configure(EntityTypeBuilder<ProductCategory> entity)
        {
            entity.ToTable("ProductCategories");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Image).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.SeoPageTitle).HasMaxLength(250);
            entity.Property(p => p.SeoAlias).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.SeoKeywords).HasMaxLength(250);
            entity.Property(p => p.SeoDescription).HasMaxLength(250);
        }
    }
}
