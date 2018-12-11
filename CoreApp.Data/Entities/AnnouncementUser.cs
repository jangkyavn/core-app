using CoreApp.Infrastructure.SharedKernel;
using System;

namespace CoreApp.Data.Entities
{
    public class AnnouncementUser : DomainEntity<int>
    {
        public string AnnouncementId { get; set; }

        public bool? HasRead { get; set; }

        public Guid CreatorId { get; set; }

        public Guid ReaderId { get; set; }

        public virtual Announcement Announcement { get; set; }
    }
}
