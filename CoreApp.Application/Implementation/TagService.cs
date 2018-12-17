using AutoMapper;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class TagService : ITagService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly IBlogTagRepository _blogTagRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagService(IProductRepository productRepository,
            IProductTagRepository productTagRepository,
            IBlogTagRepository blogTagRepository,
            ITagRepository tagRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productTagRepository = productTagRepository;
            _blogTagRepository = blogTagRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<TagViewModel> GetByIdAsync(string id)
        {
            var model = await _tagRepository.FindByIdAsync(id);
            return _mapper.Map<Tag, TagViewModel>(model);
        }

        public async Task<List<PopularTagViewModel>> GetPopularTagsAsync(int top)
        {
            return await (from t in _tagRepository.FindAll()
                         join pt in _productTagRepository.FindAll() on t.Id equals pt.TagId
                         group t by t.Id into g
                         select new PopularTagViewModel
                         {
                             Id = g.Key,
                             Name = g.FirstOrDefault(x => x.Id == g.Key.ToString()).Name,
                             Type = g.FirstOrDefault(x => x.Id == g.Key.ToString()).Type,
                             TotalCount = g.Select(x => x.Id).Count(),
                         }).Union(from t in _tagRepository.FindAll()
                                  join bt in _blogTagRepository.FindAll() on t.Id equals bt.TagId
                                  group t by t.Id into g
                                  select new PopularTagViewModel
                                  {
                                      Id = g.Key,
                                      Name = g.FirstOrDefault(x => x.Id == g.Key.ToString()).Name,
                                      Type = g.FirstOrDefault(x => x.Id == g.Key.ToString()).Type,
                                      TotalCount = g.Select(x => x.Id).Count()
                                  }).OrderByDescending(x => x.TotalCount).Take(top).ToListAsync();
        }
    }
}
