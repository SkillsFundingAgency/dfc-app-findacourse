using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.FindACourse.Data.Models
{
    public class LocationCoordinates
    {
        public bool AreValid { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }
    }
}
