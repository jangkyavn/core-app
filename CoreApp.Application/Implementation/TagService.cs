using AutoMapper;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class TagService : ITagService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagService(IProductRepository productRepository,
            IProductTagRepository productTagRepository,
            ITagRepository tagRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productTagRepository = productTagRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<TagViewModel> GetByIdAsync(string id)
        {
            var model = await _tagRepository.FindByIdAsync(id);
            return _mapper.Map<Tag, TagViewModel>(model);
        }
    }
}
