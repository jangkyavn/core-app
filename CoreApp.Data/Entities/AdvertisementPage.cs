using CoreApp.Infrastructure.SharedKernel;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class AdvertisementPage : DomainEntity<string>
    {
        public string Name { get; set; }

        public virtual ICollection<AdvertisementPosition> AdvertisementPositions { get; set; }
    }
}
