using CoreApp.Application.ViewModels;
using CoreApp.Utilities.Dtos;
using System.Collections.Generic;

namespace CoreApp.Application.Interfaces
{
    public interface IContactService
    {
        void Add(ContactDetailViewModel contactVm);

        void Update(ContactDetailViewModel contactVm);

        void Delete(string id);

        List<ContactDetailViewModel> GetAll();

        PagedResult<ContactDetailViewModel> GetAllPaging(string keyword, int page, int pageSize);

        ContactDetailViewModel GetById(string id);

        void SaveChanges();
    }
}
