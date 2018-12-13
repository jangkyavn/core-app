using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class SystemConfigConfiguration : DbEntityConfiguration<SystemConfig>
    {
        public override void Configure(EntityTypeBuilder<SystemConfig> entity)
        {
            entity.ToTable("SystemConfigs");

            entity.Property(c => c.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Value5).HasColumnType("decimal(18,2)");
        }
    }
}
