using CoreApp.Data.EF.Configurations;
using CoreApp.Data.EF.Extentions;
using CoreApp.Data.Entities;
using CoreApp.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreApp.Data.EF
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<AdvertisementPage> AdvertisementPages { get; set; }
        public DbSet<AdvertisementPosition> AdvertisementPositions { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<AnnouncementUser> AnnouncementUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ContactDetail> ContactDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Footer> Footers { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductQuantity> ProductQuantities { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<WholePrice> WholePrices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserClaim<Guid>>()
                .ToTable("AppUserClaims")
                .HasKey(x => x.Id);

            builder.Entity<IdentityRoleClaim<Guid>>()
                .ToTable("AppRoleClaims")
                .HasKey(x => x.Id);

            builder.Entity<IdentityUserLogin<Guid>>()
                .ToTable("AppUserLogins")
                .HasKey(x => x.UserId);

            builder.Entity<IdentityUserRole<Guid>>()
                .ToTable("AppUserRoles")
                .HasKey(x => new { x.RoleId, x.UserId });

            builder.Entity<IdentityUserToken<Guid>>()
                .ToTable("AppUserTokens")
                .HasKey(x => new { x.UserId });

            builder.AddConfiguration(new AdvertisementConfiguration());
            builder.AddConfiguration(new AdvertisementPageConfiguration());
            builder.AddConfiguration(new AdvertisementPositionConfiguration());
            builder.AddConfiguration(new AnnouncementConfiguration());
            builder.AddConfiguration(new AnnouncementUserConfiguration());
            builder.AddConfiguration(new AppRoleConfiguration());
            builder.AddConfiguration(new AppUserConfiguration());
            builder.AddConfiguration(new BillConfiguration());
            builder.AddConfiguration(new BillDetailConfiguration());
            builder.AddConfiguration(new BlogConfiguration());
            builder.AddConfiguration(new BlogTagConfiguration());
            builder.AddConfiguration(new ColorConfiguration());
            builder.AddConfiguration(new ContactDetailConfiguration());
            builder.AddConfiguration(new FeedbackConfiguration());
            builder.AddConfiguration(new FooterConfiguration());
            builder.AddConfiguration(new FunctionConfiguration());
            builder.AddConfiguration(new LanguageConfiguration());
            builder.AddConfiguration(new PageConfiguration());
            builder.AddConfiguration(new PermissionConfiguration());
            builder.AddConfiguration(new ProductCategoryConfiguration());
            builder.AddConfiguration(new ProductConfiguration());
            builder.AddConfiguration(new ProductImageConfiguration());
            builder.AddConfiguration(new ProductQuantityConfiguration());
            builder.AddConfiguration(new ProductTagConfiguration());
            builder.AddConfiguration(new ReviewConfiguration());
            builder.AddConfiguration(new SizeConfiguration());
            builder.AddConfiguration(new SlideConfiguration());
            builder.AddConfiguration(new SystemConfigConfiguration());
            builder.AddConfiguration(new TagConfiguration());
            builder.AddConfiguration(new WholePriceConfiguration());
        }

        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (EntityEntry item in entities)
            {
                var changedOrAddedItem = item.Entity as IDateTracking;
                if (changedOrAddedItem != null)
                {
                    if (item.State == EntityState.Added)
                    {
                        changedOrAddedItem.DateCreated = DateTime.Now;
                    }
                    else
                    {
                        changedOrAddedItem.DateModified = DateTime.Now;
                    }
                }
            }

            return base.SaveChanges();
        }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new AppDbContext(builder.Options);
        }
    }
}
