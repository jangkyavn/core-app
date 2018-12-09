using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class BillConfiguration : DbEntityConfiguration<Bill>
    {
        public override void Configure(EntityTypeBuilder<Bill> entity)
        {
            entity.ToTable("Bills");

            entity.Property(p => p.CustomerName).IsRequired().HasMaxLength(250);
            entity.Property(p => p.CustomerMobile).IsRequired().IsUnicode().HasMaxLength(50);
            entity.Property(p => p.CustomerAddress).IsRequired().HasMaxLength(500);
            entity.Property(p => p.CustomerMessage).IsRequired().HasMaxLength(500);

            entity.HasOne(p => p.AppUser)
                .WithMany(b => b.Bills)
                .HasForeignKey(p => p.CustomerId);
        }
    }
}
