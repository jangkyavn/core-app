using CoreApp.Infrastructure.SharedKernel;

namespace CoreApp.Data.Entities
{
    public class Slide : DomainEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public string GroupAlias { get; set; }
        public int? DisplayOrder { get; set; }
        public string Url { get; set; }
        public bool Status { set; get; }
    }
}
