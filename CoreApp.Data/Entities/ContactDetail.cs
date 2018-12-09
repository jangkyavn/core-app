using CoreApp.Data.Enums;
using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class ContactDetail : DomainEntity<string>, ISwitchable
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string Other { get; set; }
        public Status Status { get; set; }
    }
}
