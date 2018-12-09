using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Extensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.EF;
using CoreApp.Data.Entities;
using CoreApp.Utilities.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(
            AppDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(AppUserViewModel appUserViewModel)
        {
            var user = new AppUser()
            {
                UserName = appUserViewModel.UserName,
                Email = appUserViewModel.Email,
                FullName = appUserViewModel.FullName,
                Avatar = appUserViewModel.Avatar,
                BirthDay = appUserViewModel.BirthDay,
                PhoneNumber = appUserViewModel.PhoneNumber,
                Address = appUserViewModel.Address,
                Gender = appUserViewModel.Gender,
                Status = appUserViewModel.Status,
                DateCreated = DateTime.Now,
            };

            var result = await _userManager.CreateAsync(user, appUserViewModel.Password);
            if (result.Succeeded && appUserViewModel.Roles.Count > 0)
            {
                var appUser = await _userManager.FindByNameAsync(user.UserName);
                if (appUser != null)
                    await _userManager.AddToRolesAsync(appUser, appUserViewModel.Roles);

            }
            return true;
        }

        public Task<bool> CheckExistEmail(string email)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            await _userManager.DeleteAsync(user);
        }

        public async Task DeleteMultiAsync(List<Guid> ids)
        {
            foreach (var item in ids)
            {
                var user = await _userManager.FindByIdAsync(item.ToString());
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<List<AppUserViewModel>> GetAllAsync()
        {
            return await _userManager.Users
                .ProjectTo<AppUserViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<AppUserViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _userManager.Users;

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.FullName.ToUpper().Contains(keyword.ToUpper()) || x.UserName.ToUpper().Contains(keyword.ToUpper()) || x.Email.ToUpper().Contains(keyword.ToUpper()));
            }

            int totalRow = query.Count();

            if (pageSize == -1)
            {
                query = query.OrderBy(x => x.Status)
                    .ThenByDescending(x=> x.DateCreated);
            }
            else
            {
                query = query.OrderBy(x => x.Status)
                    .ThenByDescending(x => x.DateCreated)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);
            }

            var data = await query.Select(x => new AppUserViewModel()
            {
                Id = x.Id,
                UserName = x.UserName,
                Avatar = x.Avatar,
                Gender = x.Gender,
                Address = x.Address,
                BirthDay = x.BirthDay,
                Email = x.Email,
                FullName = x.FullName,
                PhoneNumber = x.PhoneNumber,
                DateCreated = x.DateCreated,
                Status = x.Status
            }).ToListAsync();

            var paginationSet = new PagedResult<AppUserViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<AppUserViewModel> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new AppUserViewModel();
            user.UpdateAppUserViewModel(userViewModel);
            userViewModel.Roles = roles.ToList();
            return userViewModel;
        }

        public async Task<int> GetTotalAmount()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task UpdateAsync(AppUserViewModel appUserViewModel)
        {
            var user = await _userManager.FindByIdAsync(appUserViewModel.Id.ToString());
            //Remove current roles in db
            var currentRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.AddToRolesAsync(user, appUserViewModel.Roles.Except(currentRoles).ToArray());

            if (result.Succeeded)
            {
                string[] needRemoveRoles = currentRoles.Except(appUserViewModel.Roles).ToArray();
                await RemoveRolesFromUser(user.Id.ToString(), needRemoveRoles);

                //Update user detail
                user.UserName = appUserViewModel.UserName;
                user.Email = appUserViewModel.Email;
                user.FullName = appUserViewModel.FullName;
                user.Avatar = appUserViewModel.Avatar;
                user.BirthDay = appUserViewModel.BirthDay;
                user.PhoneNumber = appUserViewModel.PhoneNumber;
                user.Address = appUserViewModel.Address;
                user.Gender = appUserViewModel.Gender;
                user.Status = appUserViewModel.Status;
                user.DateModified = DateTime.Now;

                await _userManager.UpdateAsync(user);
            }
        }

        #region Private Method
        private async Task RemoveRolesFromUser(string userId, string[] roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roleIds = _roleManager.Roles.Where(x => roles.Contains(x.Name)).Select(x => x.Id).ToList();
            List<IdentityUserRole<Guid>> userRoles = new List<IdentityUserRole<Guid>>();

            foreach (var roleId in roleIds)
            {
                userRoles.Add(new IdentityUserRole<Guid> { RoleId = roleId, UserId = user.Id });
            }

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
