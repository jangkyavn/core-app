using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ProductConfiguration : DbEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.ToTable("Products");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.CategoryId).IsRequired();
            entity.Property(p => p.Image).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.OriginalPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.PromotionPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Unit).HasMaxLength(50);
            entity.Property(p => p.SeoPageTitle).HasMaxLength(250);
            entity.Property(p => p.SeoAlias).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.SeoKeywords).HasMaxLength(250);
            entity.Property(p => p.SeoDescription).HasMaxLength(250);

            entity.HasOne(p => p.ProductCategory)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
