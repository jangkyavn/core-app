using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    class AdvertisementConfiguration : DbEntityConfiguration<Advertisement>
    {
        public override void Configure(EntityTypeBuilder<Advertisement> entity)
        {
            entity.ToTable("Advertisements");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Image).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.Url).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.PositionId).IsUnicode(false).HasMaxLength(50);

            entity.HasOne(p => p.AdvertisementPosition)
                .WithMany(b => b.Advertisements)
                .HasForeignKey(p => p.PositionId);
        }
    }
}
