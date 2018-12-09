using CoreApp.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IProductCategoryService
    {
        void Add(ProductCategoryViewModel productCategoryViewModel);

        void Update(ProductCategoryViewModel productCategoryViewModel);

        void Delete(int id);

        Task<List<ProductCategoryViewModel>> GetAllAsync();

        Task<List<ProductCategoryViewModel>> GetAllHierarchyAsync();

        Task<List<ProductCategoryViewModel>> GetAllParentAsync();

        List<ProductCategoryViewModel> GetAllByParentId(int parentId);

        ProductCategoryViewModel GetById(int id);

        Task<List<ProductCategoryViewModel>> GetHomeCategoriesAsync();

        string GetBreadcrumbs(int id);

        Task UpdateTreeNodePosition(string jsonModel);

        void Save();
    }
}
