using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class FunctionConfiguration : DbEntityConfiguration<Function>
    {
        public override void Configure(EntityTypeBuilder<Function> entity)
        {
            entity.ToTable("Functions");

            entity.Property(p => p.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.ParentId).IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.URL).IsRequired().IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.IconCss).IsUnicode(false).HasMaxLength(50);
        }
    }
}
