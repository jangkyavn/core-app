using CoreApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace CoreApp.Application.ViewModels
{
    public class FunctionViewModel
    {
        [Required]
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }

        public string ParentId { get; set; }

        [StringLength(250)]
        public string URL { get; set; }

        [StringLength(250)]
        public string IconCss { get; set; }

        public int SortOrder { get; set; }

        public Status Status { get; set; }

        public bool IsAddNew { get; set; }
    }
}
