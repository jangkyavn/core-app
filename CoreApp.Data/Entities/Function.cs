using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class Function : DomainEntity<string>, ISortable, ISwitchable
    {
        public string Name { get; set; }
        public string ParentId { get; set; }
        public string URL { get; set; }
        public string IconCss { get; set; }
        public int SortOrder { get; set; }
        public Status Status { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
