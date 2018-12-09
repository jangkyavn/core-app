using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class SlideConfiguration : DbEntityConfiguration<Slide>
    {
        public override void Configure(EntityTypeBuilder<Slide> entity)
        {
            entity.ToTable("Slides");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Image).IsRequired().IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.GroupAlias).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Url).IsUnicode(false).HasMaxLength(250);
        }
    }
}
