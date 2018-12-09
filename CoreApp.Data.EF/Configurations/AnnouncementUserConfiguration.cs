using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class AnnouncementUserConfiguration : DbEntityConfiguration<AnnouncementUser>
    {
        public override void Configure(EntityTypeBuilder<AnnouncementUser> entity)
        {
            entity.ToTable("AnnouncementUsers");

            entity.Property(p => p.AnnouncementId).IsRequired().IsUnicode(false).HasMaxLength(50);

            entity.HasOne(p => p.Announcement)
                .WithMany(b => b.AnnouncementUsers)
                .HasForeignKey(p => p.AnnouncementId);
        }
    }
}
