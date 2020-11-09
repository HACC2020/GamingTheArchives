using ArchiveSiteBackend.Api.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArchiveSiteBackend.Api.CommandLineOptions;

namespace ArchiveSiteBackend.Api.Tests
{
    public abstract class UnitTestBase
    {
        public IConfigurationRoot Configuration;

        public AzureCognitiveConfiguration AzureCognitiveConfiguration;

        protected UnitTestBase()
        {
            Configuration = Program.BuildConfiguration(
                new String[] { },
                new CommonOptions { ConfigPath = Path.Combine("..", "..", "..", "..", "ArchiveSiteBackend.Api")},
                out _
            );

            AzureCognitiveConfiguration = new AzureCognitiveConfiguration();
            Configuration.GetSection("Azure").Bind(AzureCognitiveConfiguration);
        }
    }
}
