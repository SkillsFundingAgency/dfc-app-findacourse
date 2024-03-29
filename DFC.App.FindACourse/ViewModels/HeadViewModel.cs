﻿using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class HeadViewModel
    {
        public string Title { get; set; }

        public string CanonicalUrl { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public bool IsHidden { get; set; }
    }
}