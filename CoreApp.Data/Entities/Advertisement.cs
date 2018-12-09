using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;
using System;

namespace CoreApp.Data.Entities
{
    public class Advertisement : DomainEntity<int>, IDateTracking, ISortable, ISwitchable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string PositionId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public int SortOrder { get; set; }
        public Status Status { get; set; }

        public virtual AdvertisementPosition AdvertisementPosition { get; set; }
    }
}
