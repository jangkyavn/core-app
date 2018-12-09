using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class Footer : DomainEntity<string>
    {
        public string Content { get; set; }
    }
}
