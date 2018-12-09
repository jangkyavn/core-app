using AutoMapper;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Utilities.Constants;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class FooterService : IFooterService
    {
        private readonly IFooterRepository _footerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FooterService(IFooterRepository footerRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _footerRepository = footerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(FooterViewModel footerViewModel)
        {
            var model = _mapper.Map<FooterViewModel, Footer>(footerViewModel);
            _footerRepository.Add(model);
        }

        public void Delete(string id)
        {
            var model = _footerRepository.FindById(id);
            _footerRepository.Remove(model);
        }

        public async Task<FooterViewModel> GetFooterAsync()
        {
            var model = await _footerRepository.FindByIdAsync(CommonConstants.DefaultFooterId);
            return _mapper.Map<Footer, FooterViewModel>(model);
        }

        public async Task<bool> HasDataAsync()
        {
            return await GetFooterAsync() != null;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(FooterViewModel footerViewModel)
        {
            var model = _mapper.Map<FooterViewModel, Footer>(footerViewModel);
            _footerRepository.Update(model);
        }
    }
}
