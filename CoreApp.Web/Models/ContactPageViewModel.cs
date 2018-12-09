using CoreApp.Application.ViewModels;

namespace CoreApp.Web.Models
{
    public class ContactPageViewModel
    {
        public ContactDetailViewModel Contact { set; get; }

        public FeedbackViewModel Feedback { set; get; }
    }
}
