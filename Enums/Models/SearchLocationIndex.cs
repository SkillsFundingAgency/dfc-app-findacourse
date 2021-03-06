﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.FindACourse.Data.Models
{
    [ExcludeFromCodeCoverage]
    public partial class SearchLocationIndex
    {
        public string LocationId { get; set; }

        public string LocationName { get; set; }

        public string LocalAuthorityName { get; set; }

        public string LocationAuthorityDistrict { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
