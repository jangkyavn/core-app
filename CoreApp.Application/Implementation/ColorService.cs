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
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ColorService(IColorRepository colorRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(ColorViewModel colorViewModel)
        {
            var color = new Color();
            colorViewModel.UpdateColorModel(color);
            _colorRepository.Add(color);
        }

        public void Delete(int id)
        {
            var model = _colorRepository.FindById(id);
            _colorRepository.Remove(model);
        }

        public void DeleteMultiple(List<int> ids)
        {
            var colors = new List<Color>();
            foreach (var item in ids)
            {
                colors.Add(_colorRepository.FindById(item));
            }

            _colorRepository.RemoveMultiple(colors);
        }

        public async Task<List<ColorViewModel>> GetAllAsync()
        {
            return await _colorRepository.FindAll()
               .ProjectTo<ColorViewModel>(_mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<PagedResult<ColorViewModel>> GetAllPagingAsync(string keyword, int pageIndex, int pageSize)
        {
            var query = _colorRepository.FindAll();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.ToUpper().Contains(keyword.ToUpper()) || x.Code.ToUpper().Contains(keyword.ToUpper()));
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
                .ProjectTo<ColorViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var paginationSet = new PagedResult<ColorViewModel>()
            {
                Results = data,
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        public async Task<ColorViewModel> GetById(int id)
        {
            var model = await _colorRepository.FindByIdAsync(id);
            return _mapper.Map<Color, ColorViewModel>(model);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ColorViewModel colorViewModel)
        {
            var color = new Color();
            colorViewModel.UpdateColorModel(color);
            _colorRepository.Update(color);
        }
    }
}
