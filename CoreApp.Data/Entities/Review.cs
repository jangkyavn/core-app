using CoreApp.Data.Interfaces;
using CoreApp.Infrastructure.SharedKernel;
using System;

namespace CoreApp.Data.Entities
{
    public class Review : DomainEntity<int>, IDateTracking
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual Product Product { get; set; }
    }
}
