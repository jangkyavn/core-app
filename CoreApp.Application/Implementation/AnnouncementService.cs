using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Utilities.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announcementRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnnouncementService(IAnnouncementRepository announcementRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _announcementRepository = announcementRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(AnnouncementViewModel viewModel)
        {
            var model = _mapper.Map<AnnouncementViewModel, Announcement>(viewModel);
            _announcementRepository.Add(model);
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

        public async Task<PagedResult<AnnouncementViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _announcementRepository.FindAll();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToUpper().Trim();

                query = query.Where(x => x.Title.ToUpper().Contains(keyword) || x.Content.ToUpper().Contains(keyword));
            }

            var totalRow = query.Count();
            query = query.OrderBy(x => x.Status)
                .OrderByDescending(x => x.DateCreated);

            if (pageSize != -1)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var data = await query
                .ProjectTo<AnnouncementViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

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

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
