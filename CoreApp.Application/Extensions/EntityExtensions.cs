using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;

namespace CoreApp.Application.Extensions
{
    public static class EntityExtensions
    {
        public static void UpdateAppUserViewModel(this AppUser appUser, AppUserViewModel appUserViewModel)
        {
            appUserViewModel.Id = appUser.Id;
            appUserViewModel.UserName = appUser.UserName;
            appUserViewModel.Email = appUser.Email;
            appUserViewModel.FullName = appUser.FullName;
            appUserViewModel.Avatar = appUser.Avatar;
            appUserViewModel.BirthDay = appUser.BirthDay;
            appUserViewModel.PhoneNumber = appUser.PhoneNumber;
            appUserViewModel.Address = appUser.Address;
            appUserViewModel.Gender = appUser.Gender;
            appUserViewModel.Status = appUser.Status;
            appUserViewModel.DateCreated = appUser.DateCreated;
            appUserViewModel.DateModified = appUser.DateModified;
        }

        public static void UpdateColorModel(this ColorViewModel colorViewModel, Color color)
        {
            color.Id = colorViewModel.Id;
            color.Name = colorViewModel.Name;
            color.Code = colorViewModel.Code;
        }

        public static void UpdateSizeModel(this SizeViewModel sizeViewModel, Size size)
        {
            size.Id = sizeViewModel.Id;
            size.Name = sizeViewModel.Name;
        }

        public static void UpdateFunctionModel(this FunctionViewModel functionViewModel, Function function)
        {
            function.Id = functionViewModel.Id;
            function.Name = functionViewModel.Name;
            function.ParentId = functionViewModel.ParentId;
            function.URL = functionViewModel.URL;
            function.IconCss = functionViewModel.IconCss;
            function.SortOrder = functionViewModel.SortOrder;
            function.Status = functionViewModel.Status;
        }

        public static void UpdateFunctionViewModel(this Function function, FunctionViewModel functionViewModel)
        {
            functionViewModel.Id = function.Id;
            functionViewModel.Name = function.Name;
            functionViewModel.ParentId = function.ParentId;
            functionViewModel.URL = function.URL;
            functionViewModel.IconCss = function.IconCss;
            functionViewModel.SortOrder = function.SortOrder;
            functionViewModel.Status = function.Status;
        }
    }
}
