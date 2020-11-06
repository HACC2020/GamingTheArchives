using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Configuration
{
    public class AzureCognitiveConfiguration
    {
        /// <summary>
        /// This describes the URL that your Azure account is registered to use
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// The API key used to access your Azure Cognitive Service
        /// </summary>
        public string ApiKey { get; set; }
    }
}
