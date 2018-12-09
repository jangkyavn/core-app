using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreApp.Data.EF.Configurations
{
    public class FeedbackConfiguration : DbEntityConfiguration<Feedback>
    {
        public override void Configure(EntityTypeBuilder<Feedback> entity)
        {
            entity.ToTable("Feedbacks");

            entity.Property(p => p.Name).IsRequired().HasMaxLength(250);
            entity.Property(p => p.Email).IsRequired().IsUnicode(false).HasMaxLength(250);
            entity.Property(p => p.PhoneNumber).IsRequired().IsUnicode(false).HasMaxLength(50);
            entity.Property(p => p.Message).IsRequired().HasMaxLength(500);
        }
    }
}
