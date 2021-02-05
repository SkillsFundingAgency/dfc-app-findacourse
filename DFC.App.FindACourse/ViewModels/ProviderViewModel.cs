using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.ViewModels
{
    public class ProviderViewModel
    {
        public string Name { get; set; }

        public string Website { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public double EmployerSatisfaction { get; set; }

        public bool EmployerSatisfactionSpecified { get; set; }

        public double LearnerSatisfaction { get; set; }

        public bool LearnerSatisfactionSpecified { get; set; }
    }
}
