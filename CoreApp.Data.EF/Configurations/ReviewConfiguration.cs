using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ReviewConfiguration : DbEntityConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> entity)
        {
            entity.ToTable("Reviews");

            entity.Property(p => p.Content).HasMaxLength(500);

            entity.HasOne(p => p.AppUser)
                .WithMany(b => b.Reviews)
                .HasForeignKey(p => p.UserId);

            entity.HasOne(p => p.Product)
                .WithMany(b => b.Reviews)
                .HasForeignKey(p => p.ProductId);
        }
    }
}
