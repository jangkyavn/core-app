using CoreApp.Data.Enums;

namespace CoreApp.Application.ViewModels
{
    public class ContactDetailViewModel
    {
        public string Id { get; set; }
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
