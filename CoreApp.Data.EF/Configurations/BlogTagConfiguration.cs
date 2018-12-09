using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class BlogTagConfiguration : DbEntityConfiguration<BlogTag>
    {
        public override void Configure(EntityTypeBuilder<BlogTag> entity)
        {
            entity.ToTable("BlogTags");

            entity.Property(p => p.TagId).IsRequired().IsUnicode(false).HasMaxLength(50);

            entity.HasOne(p => p.Blog)
                .WithMany(b => b.BlogTags)
                .HasForeignKey(p => p.BlogId);

            entity.HasOne(p => p.Tag)
                .WithMany(b => b.BlogTags)
                .HasForeignKey(p => p.TagId);
        }
    }
}
