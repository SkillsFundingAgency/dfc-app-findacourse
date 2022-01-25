using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class LocationCoordinates
    {
        public bool AreValid { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }
    }
}
