using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class ContactDetailConfiguration : DbEntityConfiguration<ContactDetail>
    {
        public override void Configure(EntityTypeBuilder<ContactDetail> entity)
        {
            entity.ToTable("ContactDetails");

            entity.Property(c => c.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Email).IsRequired().IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.Phone).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Address).IsRequired().HasMaxLength(500);
            entity.Property(p => p.Website).IsRequired().IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.Lng).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Lat).IsRequired().IsUnicode(false).HasMaxLength(50);
        }
    }
}
