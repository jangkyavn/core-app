using AutoMapper;
using AutoMapper.QueryableExtensions;
using CoreApp.Application.Interfaces;
using CoreApp.Application.ViewModels;
using CoreApp.Data.Entities;
using CoreApp.Data.IRepositories;
using CoreApp.Infrastructure.Interfaces;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace CoreApp.Application.Implementation
{
    public class ContactService : IContactService
    {
        private readonly IContactDetailRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContactService(
            IContactDetailRepository contactRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Add(ContactDetailViewModel pageVm)
        {
            var page = _mapper.Map<ContactDetailViewModel, ContactDetail>(pageVm);
            _contactRepository.Add(page);
        }

        public void Delete(string id)
        {
            _contactRepository.Remove(id);
        }

        public List<ContactDetailViewModel> GetAll()
        {
            return _contactRepository
                .FindAll()
                .ProjectTo<ContactDetailViewModel>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public PagedResult<ContactDetailViewModel> GetAllPaging(string keyword, int page, int pageSize)
        {
            var query = _contactRepository.FindAll();
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword));

            int totalRow = query.Count();
            var data = query.OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var paginationSet = new PagedResult<ContactDetailViewModel>()
            {
                Results = data.ProjectTo<ContactDetailViewModel>(_mapper.ConfigurationProvider).ToList(),
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }


        public ContactDetailViewModel GetById(string id)
        {
            return _mapper.Map<ContactDetail, ContactDetailViewModel>(_contactRepository.FindById(id));
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void Update(ContactDetailViewModel pageVm)
        {
            var page = Mapper.Map<ContactDetailViewModel, ContactDetail>(pageVm);
            _contactRepository.Update(page);
        }
    }
}
