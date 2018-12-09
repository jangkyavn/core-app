using CoreApp.Infrastructure.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApp.Data.Entities
{
    public class AdvertisementPosition : DomainEntity<string>
    {
        public string Name { get; set; }
        public string PageId { get; set; }

        public virtual AdvertisementPage AdvertisementPage { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
