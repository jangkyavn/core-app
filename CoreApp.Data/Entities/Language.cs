using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class Language : DomainEntity<string>, ISwitchable
    {
        public string Name { get; set; }
        public string Resources { get; set; }
        public bool IsDefault { get; set; }
        public Status Status { get; set; }
    }
}
