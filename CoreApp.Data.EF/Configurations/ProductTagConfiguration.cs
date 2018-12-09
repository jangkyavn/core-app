using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ProductTagConfiguration : DbEntityConfiguration<ProductTag>
    {
        public override void Configure(EntityTypeBuilder<ProductTag> entity)
        {
            entity.ToTable("ProductTags");

            entity.Property(c => c.TagId).IsUnicode(false).HasMaxLength(50);

            entity.HasOne(p => p.Product)
                .WithMany(b => b.ProductTags)
                .HasForeignKey(p => p.ProductId);

            entity.HasOne(p => p.Tag)
                .WithMany(b => b.ProductTags)
                .HasForeignKey(p => p.TagId);
        }
    }
}
