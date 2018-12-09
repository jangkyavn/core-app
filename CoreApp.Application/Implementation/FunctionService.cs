using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Extensions;
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
    public class FunctionService : IFunctionService
    {
        private readonly IFunctionRepository _functionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FunctionService(
            IFunctionRepository functionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _functionRepository = functionRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(FunctionViewModel functionViewModel)
        {
            var function = new Function();
            functionViewModel.UpdateFunctionModel(function);
            _functionRepository.Add(function);
        }

        public bool CheckExistedId(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            var model = _functionRepository.FindById(id);
            _functionRepository.Remove(model);
        }

        public async Task<List<FunctionViewModel>> GetAllAsync()
        {
            return await _functionRepository.FindAll()
                .OrderBy(x => x.ParentId)
                .ThenBy(x => x.SortOrder)
                .ProjectTo<FunctionViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<FunctionViewModel>> GetAllHierarchyAsync()
        {
            var model = await GetAllAsync();
            var rootFunctions = model.Where(c => c.ParentId == null);
            var items = new List<FunctionViewModel>();

            foreach (var function in rootFunctions)
            {
                //add the parent category to the item list
                items.Add(function);
                //now get all its children (separate Category in case you need recursion)
                GetSubTree(model.ToList(), function, items);
            }

            return items;
        }

        public async Task<List<FunctionViewModel>> GetAllParentAsync()
        {
            return await _functionRepository.FindAll(x => x.ParentId == null)
                .OrderBy(x => x.SortOrder)
                .ProjectTo<FunctionViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public FunctionViewModel GetById(string id)
        {
            var model = _functionRepository.FindById(id);
            var viewModel = new FunctionViewModel();
            model.UpdateFunctionViewModel(viewModel);
            return viewModel;
        }

        public async Task<List<FunctionViewModel>> GetListFunctionByPermissionAsync(Guid userId)
        {
            return await _functionRepository.GetListFunctionByPermission(userId)
                .ProjectTo<FunctionViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(FunctionViewModel functionViewModel)
        {
            var function = new Function();
            functionViewModel.UpdateFunctionModel(function);
            _functionRepository.Update(function);
        }

        public async Task UpdateTreeNodePosition(string jsonModel)
        {
            var trees = JsonConvert.DeserializeObject<List<TreeViewModel<string>>>(jsonModel);

            var sortOrder = 1;
            foreach (var tree in trees)
            {
                var model = await _functionRepository.FindByIdAsync(tree.Id);
                model.ParentId = null;
                model.SortOrder = sortOrder++;

                if (tree.Children.Count() > 0)
                {
                    await UpdateChildNode(tree);
                }

                _functionRepository.Update(model);
            }
        }

        #region Private Method
        private async Task UpdateChildNode(TreeViewModel<string> parent)
        {
            var sortOrder = 1;
            foreach (var child in parent.Children)
            {
                var model = await _functionRepository.FindByIdAsync(child.Id);
                model.ParentId = parent.Id;
                model.SortOrder = sortOrder++;

                if (child.Children.Count > 0)
                {
                    await UpdateChildNode(child);
                }

                _functionRepository.Update(model);
            }
        }

        private void GetSubTree(IEnumerable<FunctionViewModel> allFunctions,
            FunctionViewModel parent, IList<FunctionViewModel> items)
        {
            var functionsEntities = allFunctions as FunctionViewModel[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.Id);

            foreach (var cat in subFunctions)
            {
                //add this category
                items.Add(cat);
                //recursive call in case your have a hierarchy more than 1 level deep
                GetSubTree(functionsEntities, cat, items);
            }
        }
        #endregion
    }
}
