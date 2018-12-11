using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Extensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Utilities.Constants;
using CoreApp.Utilities.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IAnnouncementUserRepository _announcementUserRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnnouncementService(IAnnouncementRepository announcementRepository,
            IAnnouncementUserRepository announcementUserRepository,
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _announcementRepository = announcementRepository;
            _announcementUserRepository = announcementUserRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddAsync(AnnouncementViewModel viewModel)
        {
            var announcement = new Announcement();
            viewModel.UpdateAnnouncementModel(announcement);
            _announcementRepository.Add(announcement);

            var exceptedUsers = _userManager.Users.Where(x => x.Id != viewModel.UserId);
            foreach (var item in exceptedUsers)
            {
                var roles = await _userManager.GetRolesAsync(item);
                if (roles.Any(x => x == CommonConstants.UserRoles.AdminRole || x == CommonConstants.UserRoles.StaffRole))
                {
                    var announcementUser = new AnnouncementUser()
                    {
                        CreatorId = viewModel.UserId,
                        ReaderId = item.Id,
                        AnnouncementId = viewModel.Id,
                        HasRead = false
                    };

                    _announcementUserRepository.Add(announcementUser);
                }
            }

            _unitOfWork.Commit();
        }

        public async Task DeleteAsync(string id)
        {
            var model = await _announcementRepository.FindByIdAsync(id);
            _announcementRepository.Remove(model);
        }

        public async Task DeleteMultipleAsync(List<string> ids)
        {
            var announcements = new List<Announcement>();

            foreach (var id in ids)
            {
                var model = await _announcementRepository.FindByIdAsync(id);
                announcements.Add(model);
            }

            _announcementRepository.RemoveMultiple(announcements);
        }

        public async Task<List<AnnouncementViewModel>> GetAllAsync()
        {
            return await _announcementRepository.FindAll()
                .OrderByDescending(x => x.DateCreated)
                .ProjectTo<AnnouncementViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<AnnouncementViewModel>> GetAllPagingAsync(string keyword, Guid userId, int page, int pageSize)
        {
            var query = from a in _announcementRepository.FindAll()
                        join au in _announcementUserRepository.FindAll() on a.Id equals au.AnnouncementId
                        join u in _userManager.Users on a.UserId equals u.Id
                        where a.UserId != userId && au.ReaderId == userId
                        select new AnnouncementViewModel()
                        {
                            Id = a.Id,
                            Title = a.Title,
                            Content = a.Content,
                            FullName = u.FullName,
                            Avatar = u.Avatar,
                            DateCreated = a.DateCreated,
                            HasRead = au.HasRead.Value,
                            Status = a.Status
                        };

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToUpper().Trim();

                query = query.Where(x => x.Title.ToUpper().Contains(keyword) || x.Content.ToUpper().Contains(keyword));
            }

            var totalRow = query.Count();
            query = query.OrderByDescending(x => x.DateCreated);

            if (pageSize != -1)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var data = await query.ToListAsync();

            return new PagedResult<AnnouncementViewModel>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public async Task<AnnouncementViewModel> GetByIdAsync(string id)
        {
            var model = await _announcementRepository.FindByIdAsync(id);
            return _mapper.Map<Announcement, AnnouncementViewModel>(model);
        }

        public void MaskAsRead(Guid userId, string id)
        {
            var announcementUser = _announcementUserRepository.FindSingle(x => x.AnnouncementId == id && x.ReaderId == userId);

            if (announcementUser.HasRead == false)
            {
                announcementUser.HasRead = true;
                _announcementUserRepository.Update(announcementUser);
                _unitOfWork.Commit();
            }
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
