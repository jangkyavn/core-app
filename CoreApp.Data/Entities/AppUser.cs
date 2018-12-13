using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class AppUser : IdentityUser<Guid>, IDateTracking, ISwitchable
    {
        public string FullName { get; set; }
        public DateTime? BirthDay { get; set; }
        public string Avatar { get; set; }
        public bool Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Status Status { get; set; }

        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
