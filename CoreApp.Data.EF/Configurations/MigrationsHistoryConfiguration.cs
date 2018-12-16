using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class MigrationsHistoryConfiguration : DbEntityConfiguration<MigrationsHistory>
    {
        public override void Configure(EntityTypeBuilder<MigrationsHistory> entity)
        {
            entity.ToTable("__EFMigrationsHistory");

            entity.Property(p => p.Id).IsRequired().IsUnicode(false).HasColumnName("MigrationId").HasMaxLength(200);
            entity.Property(p => p.ProductVersion).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Test).HasMaxLength(500);
        }
    }
}
