using CoreApp.Application.Dapper.Implimentation;
using CoreApp.Application.Dapper.Interfaces;
using CoreApp.Application.Implementation;
using CoreApp.Application.Interfaces;
using CoreApp.Data.EF;
using CoreApp.Data.EF.Repositories;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Web.Authorization;
using CoreApp.Web.Helpers;
using CoreApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApp.Web.Extensions
{
    public static class DependencyInjectionExtentions
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();

            services.AddTransient(typeof(IUnitOfWork), typeof(EFUnitOfWork));
            services.AddTransient(typeof(IRepository<,>), typeof(EFRepository<,>));

            // Add User Claim
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();

            services.AddTransient<IAnnouncementRepository, AnnouncementRepository>();
            services.AddTransient<IAnnouncementUserRepository, AnnouncementUserRepository>();
            services.AddTransient<IBillDetailRepository, BillDetailRepository>();
            services.AddTransient<IBillRepository, BillRepository>();
            services.AddTransient<IBlogRepository, BlogRepository>();
            services.AddTransient<IBlogTagRepository, BlogTagRepository>();
            services.AddTransient<IColorRepository, ColorRepository>();
            services.AddTransient<IContactDetailRepository, ContactDetailRepository>();
            services.AddTransient<IFeedbackRepository, FeedbackRepository>();
            services.AddTransient<IFooterRepository, FooterRepository>();
            services.AddTransient<IFunctionRepository, FunctionRepository>();
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IProductCategoryRepository, ProductCategoryRepository>();
            services.AddTransient<IProductImageRepository, ProductImageRepository>();
            services.AddTransient<IProductQuantityRepository, ProductQuantityRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IProductTagRepository, ProductTagRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();
            services.AddTransient<ISizeRepository, SizeRepository>();
            services.AddTransient<ISlideRepository, SlideRepository>();
            services.AddTransient<ISystemConfigRepository, SystemConfigRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IWholePriceRepository, WholePriceRepository>();

            services.AddTransient<IAnnouncementService, AnnouncementService>();
            services.AddTransient<IBillService, BillService>();
            services.AddTransient<IBlogService, BlogService>();
            services.AddTransient<IColorService, ColorService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IFeedbackService, FeedbackService>();
            services.AddTransient<IFooterService, FooterService>();
            services.AddTransient<IFunctionService, FunctionService>();
            services.AddTransient<IProductCategoryService, ProductCategoryService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IReviewService, ReviewService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<ISizeService, SizeService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IReportService, ReportService>();

            services.AddTransient<IAuthorizationHandler, BaseResourceAuthorizationHandler>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IViewRenderService, ViewRenderService>();

            services.AddTransient<DbInitializer>();
        }
    }
}
