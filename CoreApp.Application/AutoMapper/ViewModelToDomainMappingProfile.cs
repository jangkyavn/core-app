using AutoMapper;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;

namespace CoreApp.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<AnnouncementViewModel, Announcement>();
            CreateMap<AppRoleViewModel, AppRole>();
            CreateMap<AppUserViewModel, AppUser>();
            CreateMap<BillDetailViewModel, BillDetail>();

            CreateMap<BillViewModel, Bill>()
              .ConstructUsing(c => new Bill(c.Id, c.CustomerName, c.CustomerAddress,
              c.CustomerMobile, c.CustomerMessage, c.BillStatus,
              c.PaymentMethod, c.DateCreated.Value, c.DateModified.Value, c.Status, c.CustomerId));

            CreateMap<BlogTagViewModel, BlogTag>();
            CreateMap<BlogViewModel, Blog>();

            CreateMap<ColorViewModel, Color>()
              .ConstructUsing(c => new Color(c.Id, c.Name, c.Code));

            CreateMap<ContactDetailViewModel, ContactDetail>();
            CreateMap<FeedbackViewModel, Feedback>();
            CreateMap<FooterViewModel, Footer>();
            CreateMap<FunctionViewModel, Function>();
            CreateMap<PermissionViewModel, Permission>();
            CreateMap<ProductCategoryViewModel, ProductCategory>();
            CreateMap<ProductImageViewModel, ProductImage>();
            CreateMap<ProductQuantityViewModel, ProductQuantity>();
            CreateMap<ProductTagViewModel, ProductTag>();
            CreateMap<ProductViewModel, Product>();
            CreateMap<ReviewViewModel, Review>();
            CreateMap<SizeViewModel, Size>();
            CreateMap<SlideViewModel, Slide>();
            CreateMap<SystemConfigViewModel, SystemConfig>();
            CreateMap<TagViewModel, Tag>();
            CreateMap<WholePriceViewModel, WholePrice>();
        }
    }
}
