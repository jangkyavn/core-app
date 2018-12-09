using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.Enums;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Utilities.Constants;
using CoreApp.Utilities.Dtos;
using CoreApp.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IBlogTagRepository _blogTagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogService(IBlogRepository blogRepository,
           IBlogTagRepository blogTagRepository,
           ITagRepository tagRepository,
           IUnitOfWork unitOfWork,
           IMapper mapper)
        {
            _blogRepository = blogRepository;
            _blogTagRepository = blogTagRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(BlogViewModel blogViewModel)
        {
            var blog = _mapper.Map<BlogViewModel, Blog>(blogViewModel);
            List<BlogTag> blogTags = new List<BlogTag>();

            if (!string.IsNullOrEmpty(blogViewModel.Tags))
            {
                string[] tags = blogViewModel.Tags.Split(',');

                foreach (var item in tags)
                {
                    var tagId = TextHelper.ConvertToUnSign(item);

                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag()
                        {
                            Id = tagId,
                            Name = item,
                            Type = CommonConstants.BlogTag
                        };

                        _tagRepository.Add(tag);
                    }

                    BlogTag blogTag = new BlogTag()
                    {
                        TagId = tagId
                    };

                    blogTags.Add(blogTag);
                }

                foreach (var blogTag in blogTags)
                {
                    blog.BlogTags.Add(blogTag);
                }
            }

            _blogRepository.Add(blog);
        }

        public async Task DeleteAsync(int id)
        {
            var model = await _blogRepository.FindByIdAsync(id);
            _blogRepository.Remove(model);
        }

        public async Task DeleteMultipleAsync(List<int> ids)
        {
            var blogs = new List<Blog>();
            foreach (var id in ids)
            {
                blogs.Add(await _blogRepository.FindByIdAsync(id));
            }

            _blogRepository.RemoveMultiple(blogs);
        }

        public async Task<List<BlogViewModel>> GetAllAsync()
        {
            return await _blogRepository
                .FindAll(c => c.BlogTags)
                .ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PagedResult<BlogViewModel>> GetAllPagingAsync(string keyword, int pageSize, int page)
        {
            var query = _blogRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword));
            }

            int totalRow = query.Count();
            query = query.OrderByDescending(x => x.DateCreated);

            if (pageSize != -1)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var data = await query
                .ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var paginationSet = new PagedResult<BlogViewModel>()
            {
                Results = data,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize,
            };

            return paginationSet;
        }

        public async Task<BlogViewModel> GetByIdAsync(int id)
        {
            return _mapper.Map<Blog, BlogViewModel>(await _blogRepository.FindByIdAsync(id));
        }

        public List<BlogViewModel> GetHotProduct(int top)
        {
            throw new NotImplementedException();
        }

        public List<BlogViewModel> GetLastest(int top)
        {
            return _blogRepository.FindAll(x => x.Status == Status.Active)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<BlogViewModel> GetList(string keyword)
        {
            var query = _blogRepository.FindAll();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.ToUpper().Contains(keyword.ToUpper()));
            }

            return query.ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider).ToList();
        }

        public List<string> GetListByName(string name)
        {
            return _blogRepository
                .FindAll(x => x.Status == Status.Active && x.Name.Contains(name))
                .Select(y => y.Name)
                .ToList();
        }

        public List<BlogViewModel> GetListByTag(string tagId, int page, int pagesize, out int totalRow)
        {
            throw new NotImplementedException();
        }

        public List<BlogViewModel> GetListPaging(int page, int pageSize, string sort, out int totalRow)
        {
            var query = _blogRepository.FindAll(x => x.Status == Status.Active);

            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;

                default:
                    query = query.OrderByDescending(x => x.DateCreated);
                    break;
            }

            totalRow = query.Count();

            return query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<TagViewModel> GetListTag(string searchText)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TagViewModel>> GetTagsByBlogIdAsync(int id)
        {
            return await _blogTagRepository.FindAll(x => x.BlogId == id, c => c.Tag)
                .Select(y => y.Tag)
                .ProjectTo<TagViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<BlogViewModel>> GetReatedBlogsAsync(int id, int top)
        {
            return await _blogRepository.FindAll(x => x.Status == Status.Active && x.Id != id)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public TagViewModel GetTag(string tagId)
        {
            return _mapper.Map<Tag, TagViewModel>(_tagRepository.FindSingle(x => x.Id == tagId));
        }

        public void IncreaseView(int id)
        {
            var product = _blogRepository.FindById(id);
            if (product.ViewCount.HasValue)
                product.ViewCount += 1;
            else
                product.ViewCount = 1;
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public List<BlogViewModel> Search(string keyword, int page, int pageSize, string sort, out int totalRow)
        {
            var query = _blogRepository.FindAll(x => x.Status == Status.Active
            && x.Name.Contains(keyword));

            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;

                default:
                    query = query.OrderByDescending(x => x.DateCreated);
                    break;
            }

            totalRow = query.Count();

            return query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<BlogViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public void Update(BlogViewModel blogViewModel)
        {
            List<BlogTag> blogTags = new List<BlogTag>();
            var blog = _mapper.Map<BlogViewModel, Blog>(blogViewModel);

            if (!string.IsNullOrEmpty(blogViewModel.Tags))
            {
                string[] tags = blogViewModel.Tags.Split(',');

                foreach (string t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);

                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag();

                        tag.Id = tagId;
                        tag.Name = t;
                        tag.Type = CommonConstants.BlogTag;

                        _tagRepository.Add(tag);
                    }

                    if (!_blogTagRepository.FindAll(x => x.BlogId == blogViewModel.Id && x.TagId == tagId).Any())
                    {
                        BlogTag blogTag = new BlogTag
                        {
                            TagId = tagId
                        };

                        blogTags.Add(blogTag);
                    }
                }

                if (blogTags.Count() > 0)
                {
                    foreach (var blogTag in blogTags)
                    {
                        blog.BlogTags.Add(blogTag);
                    }
                }
            }

            _blogRepository.Update(blog);
        }

        public PagedResult<BlogViewModel> GetBlogsPagingByTag(string tagId, int page, int pageSize)
        {
            var blogs = _blogRepository.FindAll().ToList();
            var blogTags = _blogTagRepository.FindAll().ToList();

            var query = from b in blogs
                        join bt in blogTags on b.Id equals bt.BlogId
                        where bt.TagId == tagId
                        select b;
          
            var totalRow = query.Count();
            query = query.Where(x => x.Status == Status.Active)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var viewModel = _mapper.Map<List<Blog>, List<BlogViewModel>>(query.ToList());

            return new PagedResult<BlogViewModel>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                Results = viewModel,
                RowCount = totalRow
            };
        }
    }
}
