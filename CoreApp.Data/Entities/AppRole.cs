using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public AppRole() : base()
        {
        }

        public AppRole(string name, string description) : base(name)
        {
            Description = description;
        }

        public string Description { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
