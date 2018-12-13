using AutoMapper;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;

namespace CoreApp.Application.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Announcement, AnnouncementViewModel>();
            CreateMap<AppRole, AppRoleViewModel>();
            CreateMap<AppUser, AppUserViewModel>();
            CreateMap<BillDetail, BillDetailViewModel>();
            CreateMap<Bill, BillViewModel>();
            CreateMap<BlogTag, BlogTagViewModel>();
            CreateMap<Blog, BlogViewModel>();
            CreateMap<Color, ColorViewModel>();
            CreateMap<ContactDetail, ContactDetailViewModel>();
            CreateMap<Feedback, FeedbackViewModel>();
            CreateMap<Footer, FooterViewModel>();
            CreateMap<Function, FunctionViewModel>();
            CreateMap<Permission, PermissionViewModel>();
            CreateMap<ProductCategory, ProductCategoryViewModel>().MaxDepth(1);
            CreateMap<ProductImage, ProductImageViewModel>();
            CreateMap<ProductQuantity, ProductQuantityViewModel>();
            CreateMap<ProductTag, ProductTagViewModel>();
            CreateMap<Product, ProductViewModel>().MaxDepth(1);
            CreateMap<Review, ReviewViewModel>();
            CreateMap<Size, SizeViewModel>();
            CreateMap<Slide, SlideViewModel>();
            CreateMap<SystemConfig, SystemConfigViewModel>();
            CreateMap<Tag, TagViewModel>();
            CreateMap<WholePrice, WholePriceViewModel>();
        }
    }
}
