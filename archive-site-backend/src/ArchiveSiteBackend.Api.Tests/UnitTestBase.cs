using ArchiveSiteBackend.Api.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveSiteBackend.Api.Tests
{
    public abstract class UnitTestBase
    {
        public IConfigurationRoot Configuration;

        public AzureCognitiveConfiguration AzureCognitiveConfiguration;

        public UnitTestBase()
        {
            Configuration = Program.BuildConfiguration(new String[] { }, null, out _);

            AzureCognitiveConfiguration = new AzureCognitiveConfiguration();
            Configuration.GetSection("Azure").Bind(AzureCognitiveConfiguration);
        }
    }
}
