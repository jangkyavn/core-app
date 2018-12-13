using System;

namespace CoreApp.Application.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public AppUserViewModel AppUser { get; set; }
        public ProductViewModel Product { get; set; }
    }
}
