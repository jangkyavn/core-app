using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class AppUserConfiguration : DbEntityConfiguration<AppUser>
    {
        public override void Configure(EntityTypeBuilder<AppUser> entity)
        {
            entity.ToTable("AppUsers");

            entity.Property(p => p.FullName).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Avatar).IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.Address).HasMaxLength(500);
        }
    }
}
