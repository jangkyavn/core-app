using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ProductImageConfiguration : DbEntityConfiguration<ProductImage>
    {
        public override void Configure(EntityTypeBuilder<ProductImage> entity)
        {
            entity.ToTable("ProductImages");

            entity.Property(p => p.Caption).HasMaxLength(250);
            entity.Property(p => p.Path).IsUnicode(false).HasMaxLength(250);

            entity.HasOne(p => p.Product)
                .WithMany(b => b.ProductImages)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
