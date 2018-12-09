using CoreApp.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IFunctionService
    {
        void Add(FunctionViewModel functionViewModel);

        Task<List<FunctionViewModel>> GetAllAsync();

        Task<List<FunctionViewModel>> GetAllParentAsync();

        Task<List<FunctionViewModel>> GetAllHierarchyAsync();

        Task<List<FunctionViewModel>> GetListFunctionByPermissionAsync(Guid userId);

        FunctionViewModel GetById(string id);

        void Update(FunctionViewModel functionViewModel);

        Task UpdateTreeNodePosition(string jsonModel);

        void Delete(string id);

        void Save();

        bool CheckExistedId(string id);
    }
}
