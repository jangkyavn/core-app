using CoreApp.Data.Enums;
using System;

namespace CoreApp.Application.ViewModels
{
    public class AnnouncementViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public Guid UserId { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public bool HasRead { get; set; }

        public Status Status { get; set; }

        public virtual AppUserViewModel AppUser { get; set; }
    }
}
