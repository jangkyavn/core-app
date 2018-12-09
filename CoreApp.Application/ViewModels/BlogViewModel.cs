﻿using CoreApp.Data.Enums;
using System;
using System.Collections.Generic;

namespace CoreApp.Application.ViewModels
{
    public class BlogViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public bool? HotFlag { get; set; }
        public string Tags { get; set; }
        public int? ViewCount { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string SeoPageTitle { get; set; }
        public string SeoAlias { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public Status Status { get; set; }

        public ICollection<BlogTagViewModel> BlogTags { get; set; }
    }
}
