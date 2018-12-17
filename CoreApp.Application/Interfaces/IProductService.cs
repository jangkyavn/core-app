using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreApp.Application.Interfaces
{
    public interface IProductService
    {
        void Add(ProductViewModel productViewModel);

        void Update(ProductViewModel productViewModel);

        void Delete(int id);

        void DeleteMultiple(List<int> listId);

        Task<List<ProductViewModel>> GetAllAsync();

        PagedResult<ProductViewModel> GetAllPaging(string keyword, int? categoryId, decimal? fromPrice, decimal? toPrice, string sortType, int page, int pageSize);

        ProductViewModel GetById(int id);

        Task<int> GetTotalAmount();

        void Save();

        void ImportExcel(string filePath, int categoryId);

        void AddQuantity(int productId, List<ProductQuantityViewModel> quantities);

        List<ProductQuantityViewModel> GetQuantities(int productId);

        void AddImages(int productId, string[] images);

        List<ProductImageViewModel> GetImages(int productId);

        void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices);

        List<WholePriceViewModel> GetWholePrices(int productId);

        List<ProductViewModel> GetLastest(int top);

        List<ProductViewModel> GetHotProduct(int top);

        Task<List<ProductViewModel>> GetRelatedProductsAsync(int id, int top);

        Task<List<ProductViewModel>> GetPromotionProducts(int top);

        List<ProductViewModel> GetBestSellingByCategoryId(int categoryId, int top);

        List<ProductViewModel> GetHotProductByCategoryId(int categoryId, int top);

        List<ProductViewModel> GetLatestByCategoryId(int categoryId, int top);

        List<TagViewModel> GetProductTags(int productId);

        PagedResult<ProductViewModel> GetProductsPagingByTag(string tagId, string sortType, int page, int pageSize);

        List<ProductViewModel> SuggestSearchResult(string keyword);

        bool CheckAvailability(int productId, int size, int color);

        string GetBreadcrumbs(int id);
    }
}
