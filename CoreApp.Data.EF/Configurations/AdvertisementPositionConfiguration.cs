using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class AdvertisementPositionConfiguration : DbEntityConfiguration<AdvertisementPosition>
    {
        public override void Configure(EntityTypeBuilder<AdvertisementPosition> entity)
        {
            entity.ToTable("AdvertisementPositions");

            entity.Property(c => c.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.PageId).IsUnicode(false).HasMaxLength(50);

            entity.HasOne(p => p.AdvertisementPage)
                .WithMany(b => b.AdvertisementPositions)
                .HasForeignKey(p => p.PageId);
        }
    }
}
