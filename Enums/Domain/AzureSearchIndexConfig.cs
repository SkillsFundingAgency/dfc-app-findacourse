using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class AzureSearchIndexConfig
    {
        public Uri EndpointUri { get; set; }

        public string LocationSearchIndex { get; set; }

        public string SearchServiceName { get; set; }

        public string SearchServiceAdminAPIKey { get; set; }
    }
}
