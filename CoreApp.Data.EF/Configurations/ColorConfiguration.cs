using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ColorConfiguration : DbEntityConfiguration<Color>
    {
        public override void Configure(EntityTypeBuilder<Color> entity)
        {
            entity.ToTable("Colors");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Code).IsRequired().HasMaxLength(50);
        }
    }
}
