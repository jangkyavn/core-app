using CoreApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreApp.Application.ViewModels
{
    public class BillViewModel
    {
        public int Id { get; set; }

        public Guid? CustomerId { get; set; }

        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string CustomerMobile { get; set; }

        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }

        [Display(Name = "Tin nhắn")]
        [Required(ErrorMessage = "Vui lòng nhập tin nhắn.")]
        [MaxLength(500, ErrorMessage = "Vui lòng nhập tin nhắn không vượt quá 500 ký tự.")]
        public string CustomerMessage { get; set; }

        [Display(Name = "Trạng thái hóa đơn")]
        public BillStatus BillStatus { get; set; }

        [Required]
        [Display(Name = "Phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public Status Status { set; get; }

        public AppUserViewModel AppUser { get; set; }

        public ICollection<BillDetailViewModel> BillDetails { get; set; }
    }
}
