using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class WholePriceConfiguration : DbEntityConfiguration<WholePrice>
    {
        public override void Configure(EntityTypeBuilder<WholePrice> entity)
        {
            entity.ToTable("WholePrices");

            entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.Product)
                .WithMany(b => b.WholePrices)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
