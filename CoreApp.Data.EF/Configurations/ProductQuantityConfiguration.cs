using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ProductQuantityConfiguration : DbEntityConfiguration<ProductQuantity>
    {
        public override void Configure(EntityTypeBuilder<ProductQuantity> entity)
        {
            entity.ToTable("ProductQuantities");

            entity.HasOne(p => p.Color)
                .WithMany(b => b.ProductQuantities)
                .HasForeignKey(p => p.ColorId);

            entity.HasOne(p => p.Product)
                .WithMany(b => b.ProductQuantities)
                .HasForeignKey(p => p.ProductId);

            entity.HasOne(p => p.Size)
                .WithMany(b => b.ProductQuantities)
                .HasForeignKey(p => p.SizeId);
        }
    }
}
