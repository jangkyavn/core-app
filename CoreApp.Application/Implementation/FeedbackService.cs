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
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository feedbackRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(FeedbackViewModel feedbackVm)
        {
            var page = _mapper.Map<FeedbackViewModel, Feedback>(feedbackVm);
            _feedbackRepository.Add(page);
        }

        public async Task DeleteAsync(int id)
        {
            var model = await _feedbackRepository.FindByIdAsync(id);
            _feedbackRepository.Remove(model);
        }

        public async Task DeleteMultipleAsync(List<int> ids)
        {
            var feedbacks = new List<Feedback>();

            foreach (var id in ids)
            {
                var model = await _feedbackRepository.FindByIdAsync(id);
                feedbacks.Add(model);
            }

            _feedbackRepository.RemoveMultiple(feedbacks);
        }

        public async Task<List<FeedbackViewModel>> GetAllAsync()
        {
            return await _feedbackRepository.FindAll()
                .OrderByDescending(x => x.DateCreated)
                .ProjectTo<FeedbackViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<FeedbackViewModel>> GetAllPagingAsync(string keyword, int page, int pageSize)
        {
            var query = _feedbackRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.ToUpper().Contains(keyword.ToUpper().Trim()));

            int totalRow = query.Count();
            query = query.OrderByDescending(x => x.DateCreated);

            if (pageSize != -1)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var data = await query
                .ProjectTo<FeedbackViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var paginationSet = new PagedResult<FeedbackViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<FeedbackViewModel> GetByIdAsync(int id)
        {
            return _mapper.Map<Feedback, FeedbackViewModel>(await _feedbackRepository.FindByIdAsync(id));
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(FeedbackViewModel feedbackVm)
        {
            var page = _mapper.Map<FeedbackViewModel, Feedback>(feedbackVm);
            _feedbackRepository.Update(page);
        }
    }
}
