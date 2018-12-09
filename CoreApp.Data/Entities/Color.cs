using CoreApp.Infrastructure.SharedKernel;
using System.Collections.Generic;

namespace CoreApp.Data.Entities
{
    public class Color : DomainEntity<int>
    {
        public Color()
        {
        }

        public Color(int id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }

        public Color(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<ProductQuantity> ProductQuantities { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
