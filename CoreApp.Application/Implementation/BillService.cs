using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.Enums;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Utilities.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.Application.Implementation
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _orderRepository;
        private readonly IBillDetailRepository _orderDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BillService(IBillRepository orderRepository,
            IBillDetailRepository orderDetailRepository,
            IColorRepository colorRepository,
            IProductRepository productRepository,
            ISizeRepository sizeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Create(BillViewModel billVm)
        {
            var order = _mapper.Map<BillViewModel, Bill>(billVm);
            var orderDetails = order.BillDetails;

            foreach (var detail in orderDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
            }

            order.BillDetails = orderDetails;
            _orderRepository.Add(order);
        }

        public BillDetailViewModel CreateDetail(BillDetailViewModel billDetailVm)
        {
            throw new NotImplementedException();
        }

        public void DeleteDetail(int productId, int billId, int colorId, int sizeId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BillViewModel>> GetAllAsync()
        {
            return await _orderRepository.FindAll()
                .ProjectTo<BillViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public PagedResult<BillViewModel> GetAllPaging(string startDate, string endDate, string keyword, int pageIndex, int pageSize)
        {
            var query = _orderRepository.FindAll();

            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.DateCreated.Date >= start.Date && x.DateCreated.Month >= start.Month && x.DateCreated.Year >= start.Year);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.DateCreated.Date <= end.Date && x.DateCreated.Month <= end.Month && x.DateCreated.Year <= end.Year);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.CustomerName.Contains(keyword) || x.CustomerMobile.Contains(keyword));
            }

            var totalRow = query.Count();
            query = query.OrderByDescending(x => x.DateCreated);

            if (pageSize != -1)
            {
                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }

            var data = query
                .ProjectTo<BillViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return new PagedResult<BillViewModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Results = data,
                RowCount = totalRow
            };
        }

        public List<BillDetailViewModel> GetBillDetails(int billId)
        {
            return _orderDetailRepository
                .FindAll(x => x.BillId == billId, c => c.Bill, c => c.Color, c => c.Size, c => c.Product)
                .ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public BillViewModel GetDetail(int billId)
        {
            var bill = _orderRepository.FindSingle(x => x.Id == billId);
            var billVm = _mapper.Map<Bill, BillViewModel>(bill);

            var billDetailVm = _orderDetailRepository.FindAll(x => x.BillId == billId)
                .ProjectTo<BillDetailViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            billVm.BillDetails = billDetailVm;
            return billVm;
        }

        public async Task<int> GetTotalAmount()
        {
            return await _orderRepository.FindAll().CountAsync();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(BillViewModel billVm)
        {
            //Mapping to order domain
            var order = _mapper.Map<BillViewModel, Bill>(billVm);

            //Get order Detail
            var newDetails = order.BillDetails;

            //new details added
            var addedDetails = newDetails.Where(x => x.Id == 0).ToList();

            //get updated details
            var updatedDetails = newDetails.Where(x => x.Id != 0).ToList();

            //Existed details
            var existedDetails = _orderDetailRepository.FindAll(x => x.BillId == billVm.Id);

            //Clear db
            order.BillDetails.Clear();

            foreach (var detail in updatedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.Price = product.Price;
                _orderDetailRepository.Update(detail);
            }

            foreach (var detail in addedDetails)
            {
                var product = _productRepository.FindById(detail.ProductId);
                detail.BillId = billVm.Id;
                detail.Price = product.Price;
                _orderDetailRepository.Add(detail);
            }

            foreach (var item in existedDetails)
            {
                if (!updatedDetails.Any(x => x.Id == item.Id))
                {
                    _orderDetailRepository.Remove(item);
                }
            }

            _orderRepository.Update(order);
        }

        public void UpdateStatus(int orderId, BillStatus status)
        {
            var order = _orderRepository.FindById(orderId);
            order.BillStatus = status;
            _orderRepository.Update(order);
        }
    }
}
