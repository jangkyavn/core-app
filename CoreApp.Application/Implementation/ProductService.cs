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
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly IProductQuantityRepository _productQuantityRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IWholePriceRepository _wholePriceRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository,
            IProductCategoryRepository productCategoryRepository,
            IProductTagRepository productTagRepository,
            IProductQuantityRepository productQuantityRepository,
            IProductImageRepository productImageRepository,
            IWholePriceRepository wholePriceRepository,
            ITagRepository tagRepository,
            IBillDetailRepository billDetailRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _productTagRepository = productTagRepository;
            _productQuantityRepository = productQuantityRepository;
            _productImageRepository = productImageRepository;
            _wholePriceRepository = wholePriceRepository;
            _tagRepository = tagRepository;
            _billDetailRepository = billDetailRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(ProductViewModel productViewModel)
        {
            List<ProductTag> productTags = new List<ProductTag>();
            var product = _mapper.Map<ProductViewModel, Product>(productViewModel);

            if (!string.IsNullOrEmpty(productViewModel.Tags))
            {
                string[] tags = productViewModel.Tags.Split(',');

                foreach (var t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);

                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag()
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };

                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagId
                    };

                    productTags.Add(productTag);
                }

                foreach (var productTag in productTags)
                {
                    product.ProductTags.Add(productTag);
                }
            }

            _productRepository.Add(product);
        }

        public void AddImages(int productId, string[] images)
        {
            _productImageRepository.RemoveMultiple(_productImageRepository
                                                    .FindAll(x => x.ProductId == productId)
                                                    .ToList());

            foreach (var image in images)
            {
                _productImageRepository.Add(new ProductImage()
                {
                    Path = image,
                    ProductId = productId,
                    Caption = string.Empty
                });
            }
        }

        public void AddQuantity(int productId, List<ProductQuantityViewModel> quantities)
        {
            _productQuantityRepository.RemoveMultiple(_productQuantityRepository
                                                    .FindAll(x => x.ProductId == productId)
                                                    .ToList());

            foreach (var quantity in quantities)
            {
                _productQuantityRepository.Add(new ProductQuantity()
                {
                    ProductId = productId,
                    ColorId = quantity.ColorId,
                    SizeId = quantity.SizeId,
                    Quantity = quantity.Quantity
                });
            }
        }

        public void AddWholePrice(int productId, List<WholePriceViewModel> wholePrices)
        {
            _wholePriceRepository.RemoveMultiple(_wholePriceRepository
                                                .FindAll(x => x.ProductId == productId)
                                                .ToList());

            foreach (var wholePrice in wholePrices)
            {
                _wholePriceRepository.Add(new WholePrice()
                {
                    ProductId = productId,
                    FromQuantity = wholePrice.FromQuantity,
                    ToQuantity = wholePrice.ToQuantity,
                    Price = wholePrice.Price
                });
            }
        }

        public bool CheckAvailability(int productId, int size, int color)
        {
            var quantity = _productQuantityRepository.FindSingle(x => x.ColorId == color && x.SizeId == size && x.ProductId == productId);
            if (quantity == null)
                return false;
            return quantity.Quantity > 0;
        }

        public void Delete(int id)
        {
            _productRepository.Remove(id);
        }

        public void DeleteMultiple(List<int> listId)
        {
            List<Product> products = new List<Product>();
            foreach (var item in listId)
            {
                products.Add(_productRepository.FindById(item));
            }

            _productRepository.RemoveMultiple(products);
        }

        public async Task<List<ProductViewModel>> GetAllAsync()
        {
            return await _productRepository.FindAll()
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public PagedResult<ProductViewModel> GetAllPaging(string keyword, int? categoryId, string sortType, int page, int pageSize)
        {
            var query = _productRepository.FindAll(x => x.Status == Status.Active, x => x.ProductCategory);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => TextHelper.ConvertToUnSign(x.Name.ToUpper()).Contains(TextHelper.ConvertToUnSign(keyword.ToUpper().Trim())));
            }

            if (categoryId.HasValue)
            {
                var products = new List<Product>();
                LoadProductByCategory(categoryId.Value, products);

                query = products.AsQueryable();
            }

            int totalRow = query.Count();
            query = query.OrderBy(x => x.Status)
                    .ThenByDescending(x => x.DateCreated);

            switch (sortType)
            {
                case "latest":
                    query = query.OrderByDescending(x => x.DateCreated);
                    break;
                case "oldest":
                    query = query.OrderBy(x => x.DateCreated);
                    break;
                case "price-ascending":
                    query = query.OrderBy(x => x.Price);
                    break;
                case "price-descending":
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case "name-a-z":
                    query = query.OrderBy(x => x.Name);
                    break;
                case "name-z-a":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                default:
                    break;
            }

            if (pageSize != -1)
            {
                query = query.Skip((page - 1) * pageSize).Take(pageSize);
            }

            var viewModel = _mapper.Map<List<Product>, List<ProductViewModel>>(query.ToList());

            return new PagedResult<ProductViewModel>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                Results = viewModel,
                RowCount = totalRow
            };
        }

        public List<ProductViewModel> GetBestSellingByCategoryId(int categoryId, int top)
        {
            var allCategories = _productCategoryRepository.FindAll().ToList();
            var categories = allCategories.Where(x => x.ParentId == categoryId);
            List<Product> products = new List<Product>();

            foreach (var item in categories)
            {
                if (allCategories.Any(x => x.ParentId == item.Id))
                {
                    var categoryChildrens = allCategories.Where(x => x.ParentId == item.Id);

                    foreach (var child in categoryChildrens)
                    {
                        var productsByCategory = _productRepository.FindAll(x => x.CategoryId == child.Id);
                        products.AddRange(productsByCategory);
                    }
                }
                else
                {
                    var productsByCategory = _productRepository.FindAll(x => x.CategoryId == item.Id);
                    products.AddRange(productsByCategory);
                }
            }

            var groupByQuantity = (from p in products
                                   join bd in _billDetailRepository.FindAll() on p.Id equals bd.ProductId
                                   group bd by p.Id into g
                                   select new
                                   {
                                       ProductId = g.Key,
                                       TotalQuantity = g.Sum(x => x.Quantity)
                                   }).ToList();

            var data = from p in products
                       join g in groupByQuantity on p.Id equals g.ProductId
                       where p.Status == Status.Active
                       orderby g.TotalQuantity descending, p.DateCreated descending
                       select p;

            return _mapper.Map<List<Product>, List<ProductViewModel>>(data.Take(top).ToList());
        }

        public string GetBreadcrumbs(int id)
        {
            string results = "<li class='home'> <a title='Trở về trang chủ' href='/trang-chu.html'>Trang chủ</a><span>&raquo;</span></li>";

            var categories = _productCategoryRepository.FindAll().ToList();
            var product = _productRepository.FindById(id);
            var category = _productCategoryRepository.FindById(product.CategoryId);
            var url = $"/{category.SeoAlias}-c.{category.Id}.html";

            if (categories.Any(x => x.Id == category.ParentId))
            {
                var parent = _productCategoryRepository.FindById(category.ParentId.Value);
                var urlParent = $"/{parent.SeoAlias}-c.{parent.Id}.html";

                if (categories.Any(x => x.Id == parent.ParentId))
                {
                    var grandParent = _productCategoryRepository.FindById(parent.ParentId.Value);
                    var urlGrandParent = $"/{grandParent.SeoAlias}-c.{grandParent.Id}.html";

                    if (grandParent.ParentId == null)
                    {
                        results += $"<li class=''> <a title='Đi tới {grandParent.Name}' href='{urlGrandParent}'>{grandParent.Name}</a><span>&raquo;</span></li>";
                    }

                    results += $"<li class=''> <a title='Đi tới {parent.Name}' href='{urlParent}'>{parent.Name}</a><span>&raquo;</span></li>";
                }

                results += $"<li class=''> <a title='Đi tới {category.Name}' href='{url}'>{category.Name}</a><span>&raquo;</span></li>";
            }

            results += $"<li><strong>{product.Name}</strong></li>";

            return results;
        }

        public ProductViewModel GetById(int id)
        {
            var model = _productRepository.FindById(id);

            return _mapper.Map<Product, ProductViewModel>(model);
        }

        public List<ProductViewModel> GetHotProduct(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active && x.HotFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ProductViewModel> GetHotProductByCategoryId(int categoryId, int top)
        {
            var allCategories = _productCategoryRepository.FindAll().ToList();
            var categories = allCategories.Where(x => x.ParentId == categoryId);
            List<Product> products = new List<Product>();

            foreach (var item in categories)
            {
                if (allCategories.Any(x => x.ParentId == item.Id))
                {
                    var categoryChildrens = allCategories.Where(x => x.ParentId == item.Id);

                    foreach (var child in categoryChildrens)
                    {
                        var productsByCategory = _productRepository.FindAll(x => x.CategoryId == child.Id);
                        products.AddRange(productsByCategory);
                    }
                }
                else
                {
                    var productsByCategory = _productRepository.FindAll(x => x.CategoryId == item.Id);
                    products.AddRange(productsByCategory);
                }
            }

            var data = products.Where(x => x.Status == Status.Active && x.HotFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ToList();

            return _mapper.Map<List<Product>, List<ProductViewModel>>(data);
        }

        public List<ProductImageViewModel> GetImages(int productId)
        {
            return _productImageRepository
                .FindAll(x => x.ProductId == productId)
                .ProjectTo<ProductImageViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ProductViewModel> GetLastest(int top)
        {
            return _productRepository.FindAll(x => x.Status == Status.Active)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ProductViewModel> GetLatestByCategoryId(int categoryId, int top)
        {
            var allCategories = _productCategoryRepository.FindAll().ToList();
            var categories = allCategories.Where(x => x.ParentId == categoryId);
            List<Product> products = new List<Product>();

            foreach (var item in categories)
            {
                if (allCategories.Any(x => x.ParentId == item.Id))
                {
                    var categoryChildrens = allCategories.Where(x => x.ParentId == item.Id);

                    foreach (var child in categoryChildrens)
                    {
                        var productsByCategory = _productRepository.FindAll(x => x.CategoryId == child.Id);
                        products.AddRange(productsByCategory);
                    }
                }
                else
                {
                    var productsByCategory = _productRepository.FindAll(x => x.CategoryId == item.Id);
                    products.AddRange(productsByCategory);
                }
            }

            var data = products.Where(x => x.Status == Status.Active)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ToList();

            return _mapper.Map<List<Product>, List<ProductViewModel>>(data);
        }

        public PagedResult<ProductViewModel> GetProductsPagingByTag(string tagId, string sortType, int page, int pageSize)
        {
            var products = _productRepository.FindAll().ToList();
            var productTags = _productTagRepository.FindAll().ToList();

            var query = from p in products
                        join pt in productTags on p.Id equals pt.ProductId
                        where pt.TagId == tagId
                        select p;

            switch (sortType)
            {
                case "latest":
                    query = query.OrderByDescending(x => x.DateCreated);
                    break;
                case "oldest":
                    query = query.OrderBy(x => x.DateCreated);
                    break;
                case "price-ascending":
                    query = query.OrderBy(x => x.Price);
                    break;
                case "price-descending":
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case "name-a-z":
                    query = query.OrderBy(x => x.Name);
                    break;
                case "name-z-a":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                default:
                    break;
            }

            var totalRow = query.Count();
            query = query.Where(x => x.Status == Status.Active)
                .Skip((page - 1) * pageSize).Take(pageSize);

            var viewModel = _mapper.Map<List<Product>, List<ProductViewModel>>(query.ToList());

            return new PagedResult<ProductViewModel>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                Results = viewModel,
                RowCount = totalRow
            };
        }

        public List<TagViewModel> GetProductTags(int productId)
        {
            var tags = _tagRepository.FindAll();
            var productTags = _productTagRepository.FindAll();

            var query = from t in tags
                        join pt in productTags
                        on t.Id equals pt.TagId
                        where pt.ProductId == productId
                        select new TagViewModel()
                        {
                            Id = t.Id,
                            Name = t.Name
                        };

            return query.ToList();
        }

        public async Task<List<ProductViewModel>> GetPromotionProducts(int top)
        {
            return await _productRepository.FindAll(x => x.Status == Status.Active && x.PromotionPrice.HasValue)
                .OrderByDescending(x => x.PromotionPrice.Value)
                .Take(top)
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public List<ProductQuantityViewModel> GetQuantities(int productId)
        {
            return _productQuantityRepository
                .FindAll(x => x.ProductId == productId)
                .ProjectTo<ProductQuantityViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public async Task<List<ProductViewModel>> GetRelatedProductsAsync(int id, int top)
        {
            var product = _productRepository.FindById(id);
            return await _productRepository.FindAll(x => x.Status == Status.Active
                && x.Id != id && x.CategoryId == product.CategoryId)
                .OrderByDescending(x => x.DateCreated)
                .Take(top)
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetTotalAmount()
        {
            return await _productRepository.FindAll().CountAsync();
        }

        public List<WholePriceViewModel> GetWholePrices(int productId)
        {
            return _wholePriceRepository
                .FindAll(x => x.ProductId == productId)
                .ProjectTo<WholePriceViewModel>(_mapper.ConfigurationProvider)
                .ToList();

        }

        public void ImportExcel(string filePath, int categoryId)
        {
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                Product product;

                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    product = new Product();
                    product.CategoryId = categoryId;

                    product.Name = workSheet.Cells[i, 1].Value.ToString();

                    product.Description = workSheet.Cells[i, 2].Value.ToString();

                    product.Content = workSheet.Cells[i, 3].Value.ToString();

                    decimal.TryParse(workSheet.Cells[i, 4].Value.ToString(), out var originalPrice);
                    product.OriginalPrice = originalPrice;

                    decimal.TryParse(workSheet.Cells[i, 5].Value.ToString(), out var price);
                    product.Price = price;

                    decimal.TryParse(workSheet.Cells[i, 6].Value.ToString(), out var promotionPrice);
                    product.PromotionPrice = promotionPrice;

                    product.Unit = workSheet.Cells[i, 7].Value.ToString();

                    product.SeoPageTitle = workSheet.Cells[i, 8].Value.ToString();
                    product.SeoKeywords = workSheet.Cells[i, 9].Value.ToString();
                    product.SeoDescription = workSheet.Cells[i, 10].Value.ToString();

                    bool.TryParse(workSheet.Cells[i, 11].Value.ToString(), out var hotFlag);
                    product.HotFlag = hotFlag;

                    bool.TryParse(workSheet.Cells[i, 12].Value.ToString(), out var status);
                    product.Status = status ? Status.Active : Status.InActive;

                    _productRepository.Add(product);
                }
            }
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public List<ProductViewModel> SuggestSearchResult(string keyword)
        {
            var query = _productRepository.FindAll(x => x.Status == Status.Active);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(delegate (Product p)
                {
                    if (TextHelper.ConvertToUnSign(p.Name.ToUpper()).IndexOf(keyword.ToUpper().Trim(), StringComparison.CurrentCultureIgnoreCase) >= 0)
                        return true;
                    else
                        return false;
                }).AsQueryable();
            }

            return _mapper.Map<List<Product>, List<ProductViewModel>>(query.ToList());
        }

        public void Update(ProductViewModel productViewModel)
        {
            List<ProductTag> productTags = new List<ProductTag>();
            var product = _mapper.Map<ProductViewModel, Product>(productViewModel);

            if (!string.IsNullOrEmpty(productViewModel.Tags))
            {
                string[] tags = productViewModel.Tags.Split(',');

                foreach (string t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);

                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag();

                        tag.Id = tagId;
                        tag.Name = t;
                        tag.Type = CommonConstants.ProductTag;

                        _tagRepository.Add(tag);
                    }

                    if (!_productTagRepository.FindAll(x => x.ProductId == productViewModel.Id && x.TagId == tagId).Any())
                    {
                        ProductTag productTag = new ProductTag
                        {
                            TagId = tagId
                        };

                        productTags.Add(productTag);
                    }
                }

                if (productTags.Count() > 0)
                {
                    foreach (var productTag in productTags)
                    {
                        product.ProductTags.Add(productTag);
                    }
                }
            }

            _productRepository.Update(product);
        }

        #region Private Method
        private void LoadProductByCategory(int categoryId, List<Product> products)
        {
            var catsAll = _productCategoryRepository.FindAll().ToList();

            if (catsAll.Where(x => x.ParentId == categoryId).Count() > 0)
            {
                foreach (var item in catsAll.Where(x => x.ParentId == categoryId))
                {
                    if (catsAll.Any(x => x.ParentId == item.Id))
                    {
                        var categories = catsAll.Where(x => x.ParentId == item.Id);

                        foreach (var category in categories)
                        {
                            var productsByCategoryId = _productRepository
                                .FindAll(x => x.CategoryId == category.Id, x => x.ProductCategory)
                                .OrderBy(x => x.Status)
                                .ThenByDescending(x => x.DateCreated);

                            foreach (var product in productsByCategoryId)
                            {
                                products.Add(product);
                            }
                        }
                    }
                    else
                    {
                        var productsByCategoryId = _productRepository
                            .FindAll(x => x.CategoryId == item.Id, x => x.ProductCategory)
                            .OrderBy(x => x.Status)
                            .ThenByDescending(x => x.DateCreated);

                        foreach (var product in productsByCategoryId)
                        {
                            products.Add(product);
                        }
                    }
                }
            }
            else
            {
                var productsByCategoryId = _productRepository
                    .FindAll(x => x.CategoryId == categoryId, x => x.ProductCategory)
                    .OrderBy(x => x.Status)
                    .ThenByDescending(x => x.DateCreated);

                foreach (var item in productsByCategoryId)
                {
                    products.Add(item);
                }
            }
        }
        #endregion
    }
}
