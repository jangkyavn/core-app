using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Extensions;
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
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SizeService(ISizeRepository sizeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _sizeRepository = sizeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(SizeViewModel sizeViewModel)
        {
            var size = new Size();
            sizeViewModel.UpdateSizeModel(size);
            _sizeRepository.Add(size);
        }

        public void Delete(int id)
        {
            var model = _sizeRepository.FindById(id);
            _sizeRepository.Remove(model);
        }

        public void DeleteMultiple(List<int> ids)
        {
            var sizes = new List<Size>();
            foreach (var item in ids)
            {
                sizes.Add(_sizeRepository.FindById(item));
            }

            _sizeRepository.RemoveMultiple(sizes);
        }

        public async Task<List<SizeViewModel>> GetAllAsync()
        {
            return await _sizeRepository.FindAll()
               .ProjectTo<SizeViewModel>(_mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<PagedResult<SizeViewModel>> GetAllPagingAsync(string keyword, int pageIndex, int pageSize)
        {
            var query = _sizeRepository.FindAll();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.ToUpper().Contains(keyword.ToUpper()));
            }

            int totalRow = query.Count();

            if (pageSize != -1)
            {
                query = query.OrderBy(x => x.Name)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            var data = await query
                .ProjectTo<SizeViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var paginationSet = new PagedResult<SizeViewModel>()
            {
                Results = data,
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<SizeViewModel> GetById(int id)
        {
            var model = await _sizeRepository.FindByIdAsync(id);
            return _mapper.Map<Size, SizeViewModel>(model);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(SizeViewModel sizeViewModel)
        {
            var size = new Size();
            sizeViewModel.UpdateSizeModel(size);
            _sizeRepository.Update(size);
        }
    }
}
