using CoreApp.Infrastructure.SharedKernel;
using System;

namespace CoreApp.Data.Entities
{
    public class Permission : DomainEntity<int>
    {
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public string FunctionId { get; set; }
        public Guid RoleId { get; set; }

        public virtual Function Function { get; set; }
        public virtual AppRole AppRole { get; set; }
    }
}
