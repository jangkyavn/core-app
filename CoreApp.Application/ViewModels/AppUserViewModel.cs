using CoreApp.Data.Enums;
using System;
using System.Collections.Generic;

namespace CoreApp.Application.ViewModels
{
    public class AppUserViewModel
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDay { get; set; }
        public string PhoneNumber { get; set; }
        public string Avatar { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Status Status { get; set; }

        public ICollection<string> Roles { get; set; } = new List<string>();
        //public ICollection<Announcement> Announcements { get; set; }
        public ICollection<BillViewModel> Bills { get; set; }
    }
}
