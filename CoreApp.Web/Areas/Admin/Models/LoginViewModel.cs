using System.ComponentModel.DataAnnotations;

namespace CoreApp.Web.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
