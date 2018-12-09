using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class Page : DomainEntity<int>, ISwitchable
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Content { get; set; }
        public Status Status { get; set; }
    }
}
