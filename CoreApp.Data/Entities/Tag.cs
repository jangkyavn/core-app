using CoreApp.Infrastructure.SharedKernel;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class Tag : DomainEntity<string>
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<BlogTag> BlogTags { get; set; }
        public virtual ICollection<ProductTag> ProductTags { get; set; }
    }
}
