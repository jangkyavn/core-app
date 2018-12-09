using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class PageConfiguration : DbEntityConfiguration<Page>
    {
        public override void Configure(EntityTypeBuilder<Page> entity)
        {
            entity.ToTable("Pages");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Alias).IsRequired().IsUnicode(false).HasMaxLength(250);
        }
    }
}
