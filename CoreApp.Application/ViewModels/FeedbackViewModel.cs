﻿using CoreApp.Data.Enums;
using System;

namespace CoreApp.Application.ViewModels
{
    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public Status Status { get; set; }
    }
}
