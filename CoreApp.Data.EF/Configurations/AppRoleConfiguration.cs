using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class AppRoleConfiguration : DbEntityConfiguration<AppRole>
    {
        public override void Configure(EntityTypeBuilder<AppRole> entity)
        {
            entity.ToTable("AppRoles");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Description).HasMaxLength(250);
        }
    }
}
