using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class BillDetailConfiguration : DbEntityConfiguration<BillDetail>
    {
        public override void Configure(EntityTypeBuilder<BillDetail> entity)
        {
            entity.ToTable("BillDetails");

            entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");

            entity.HasOne(p => p.Bill)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(p => p.BillId);

            entity.HasOne(p => p.Color)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(p => p.ColorId);

            entity.HasOne(p => p.Size)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(p => p.SizeId);

            entity.HasOne(p => p.Product)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
