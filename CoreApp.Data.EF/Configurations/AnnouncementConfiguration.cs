using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class AnnouncementConfiguration : DbEntityConfiguration<Announcement>
    {
        public override void Configure(EntityTypeBuilder<Announcement> entity)
        {
            entity.ToTable("Announcements");

            entity.Property(c => c.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Content).HasMaxLength(500);

            entity.HasOne(p => p.AppUser)
                .WithMany(b => b.Announcements)
                .HasForeignKey(p => p.UserId);
        }
    }
}
