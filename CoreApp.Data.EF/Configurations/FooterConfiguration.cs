using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class FooterConfiguration : DbEntityConfiguration<Footer>
    {
        public override void Configure(EntityTypeBuilder<Footer> entity)
        {
            entity.ToTable("Footers");

            entity.Property(c => c.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Content).IsRequired();
        }
    }
}
