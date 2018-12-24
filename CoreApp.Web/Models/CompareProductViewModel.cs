using CoreApp.Application.ViewModels;

namespace CoreApp.Web.Models
{
    public class CompareProductViewModel
    {
        public ProductViewModel Product { get; set; }

        public int RatingAverage { get; set; }

        public int RatingTotal { get; set; }
    }
}
