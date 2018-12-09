using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class PermissionConfiguration : DbEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> entity)
        {
            entity.ToTable("Permissions");

            entity.Property(p => p.FunctionId).IsRequired().IsUnicode(false).HasMaxLength(50);

            entity.HasOne(p => p.Function)
                .WithMany(b => b.Permissions)
                .HasForeignKey(p => p.FunctionId);

            entity.HasOne(p => p.AppRole)
                .WithMany(b => b.Permissions)
                .HasForeignKey(p => p.RoleId);
        }
    }
}
