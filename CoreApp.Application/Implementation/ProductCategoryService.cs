using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductCategoryService(
            IProductCategoryRepository productCategoryRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _productCategoryRepository = productCategoryRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(ProductCategoryViewModel productCategoryViewModel)
        {
            var productCategory = _mapper.Map<ProductCategoryViewModel, ProductCategory>(productCategoryViewModel);

            _productCategoryRepository.Add(productCategory);
        }

        public void Delete(int id)
        {
            _productCategoryRepository.Remove(id);
        }

        public async Task<List<ProductCategoryViewModel>> GetAllAsync()
        {
            return await _productCategoryRepository.FindAll()
                .OrderBy(x => x.ParentId)
                .ThenBy(x => x.SortOrder)
                .ProjectTo<ProductCategoryViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<ProductCategoryViewModel>> GetAllHierarchyAsync()
        {
            List<ProductCategoryViewModel> items = new List<ProductCategoryViewModel>();

            //get all of them from DB
            var allCategorys = await GetAllAsync();
            //get parent categories
            var parentCategorys = allCategorys.Where(c => c.ParentId == null);

            foreach (var cat in parentCategorys)
            {
                //add the parent category to the item list
                items.Add(new ProductCategoryViewModel
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    ParentId = cat.ParentId
                });
                //now get all its children (separate Category in case you need recursion)
                GetSubTree(allCategorys.ToList(), cat, items);
            }

            return items;
        }
        
        public ProductCategoryViewModel GetById(int id)
        {
            var model = _productCategoryRepository.FindById(id);

            return _mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
        }

        public async Task<List<ProductCategoryViewModel>> GetHomeCategoriesAsync()
        {
            return await _productCategoryRepository.FindAll(x => x.ParentId == null)
                .OrderBy(x => x.SortOrder)
                .ProjectTo<ProductCategoryViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(ProductCategoryViewModel productCategoryViewModel)
        {
            var model = _mapper.Map<ProductCategoryViewModel, ProductCategory>(productCategoryViewModel);

            _productCategoryRepository.Update(model);
        }

        public async Task UpdateTreeNodePosition(string jsonModel)
        {
            var trees = JsonConvert.DeserializeObject<List<TreeViewModel<int>>>(jsonModel);

            var sortOrder = 1;
            foreach (var tree in trees)
            {
                var model = await _productCategoryRepository.FindByIdAsync(tree.Id);
                model.ParentId = null;
                model.SortOrder = sortOrder++;

                if (tree.Children.Count() > 0)
                {
                    await UpdateChildNode(tree);
                }

                _productCategoryRepository.Update(model);
            }
        }

        public List<ProductCategoryViewModel> GetAllByParentId(int parentId)
        {
            return _productCategoryRepository
                .FindAll(x => x.ParentId == parentId)
                .OrderBy(x => x.SortOrder)
                .ProjectTo<ProductCategoryViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public async Task<List<ProductCategoryViewModel>> GetAllParentAsync()
        {
            var data = await _productCategoryRepository.FindAll()
                .OrderBy(x => x.SortOrder)
                .ProjectTo<ProductCategoryViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var parents = new List<ProductCategoryViewModel>();
            foreach (var item in data.Where(x => x.ParentId == null))
            {
                parents.Add(item);
                GetChildNode(data, item, parents);
            }

            return parents;
        }

        public string GetBreadcrumbs(int id)
        {
            string results = "<li class='home'> <a title='Trở về trang chủ' href='/trang-chu.html'>Trang chủ</a><span>&raquo;</span></li>";

            var categories = _productCategoryRepository.FindAll().ToList();
            var category = _productCategoryRepository.FindById(id);
            if (category.ParentId == null)
            {
                results += $"<li><strong>{category.Name}</strong></li>";
            }
            else
            {
                if (category.ParentId != null && categories.Any(x => x.ParentId == category.Id))
                {
                    var parent = _productCategoryRepository.FindSingle(x => x.Id == category.ParentId);
                    var url = $"/{parent.SeoAlias}-c.{parent.Id}.html";

                    results += $"<li class=''> <a title='Đi tới {parent.Name}' href='{url}'>{parent.Name}</a><span>&raquo;</span></li>" +
                        $"<li><strong>{category.Name}</strong></li> ";
                }
                else
                {
                    var parent = _productCategoryRepository.FindSingle(x => x.Id == category.ParentId);
                    var grandParent = _productCategoryRepository.FindSingle(x => x.Id == parent.ParentId);

                    var urlGrandParent = $"/{grandParent.SeoAlias}-c.{grandParent.Id}.html";
                    var urlParent = $"/{parent.SeoAlias}-c.{parent.Id}.html";

                    results += $"<li class=''> <a title='Đi tới {grandParent.Name}' href='{urlGrandParent}'>{grandParent.Name}</a><span>&raquo;</span></li>" +
                        $"<li class=''> <a title='Đi tới {parent.Name}' href='{urlParent}'>{parent.Name}</a><span>&raquo;</span></li>" +
                        $"<li><strong>{category.Name}</strong></li> ";
                }
            }

            return results;
        }

        #region Private Method
        private async Task UpdateChildNode(TreeViewModel<int> parent)
        {
            var sortOrder = 1;
            foreach (var child in parent.Children)
            {
                var model = await _productCategoryRepository.FindByIdAsync(child.Id);
                model.ParentId = parent.Id;
                model.SortOrder = sortOrder++;

                if (child.Children.Count > 0)
                {
                    await UpdateChildNode(child);
                }

                _productCategoryRepository.Update(model);
            }
        }

        private void GetSubTree(IList<ProductCategoryViewModel> allCats, ProductCategoryViewModel parent, IList<ProductCategoryViewModel> items)
        {
            var subCats = allCats.Where(c => c.ParentId == parent.Id);
            foreach (var cat in subCats)
            {
                //add this category
                items.Add(new ProductCategoryViewModel
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    ParentId = cat.ParentId
                });
                //recursive call in case your have a hierarchy more than 1 level deep
                GetSubTree(allCats, cat, items);
            }
        }

        private void GetChildNode(List<ProductCategoryViewModel> data, ProductCategoryViewModel item, List<ProductCategoryViewModel> parents)
        {
            var subCats = data.Where(c => c.ParentId == item.Id);

            foreach (var cat in subCats)
            {
                //add this category
                parents.Add(cat);
            }
        }
        #endregion
    }
}
