using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class MigrationsHistory : DomainEntity<string>
    {
        public string ProductVersion { get; set; }
        public string Test { get; set; }
    }
}
