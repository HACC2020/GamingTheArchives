using ArchiveSiteBackend.Api.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Policy;
using Xunit;

namespace ArchiveSiteBackend.Api.Tests
{
    public class AppSettingsUnitTest : UnitTestBase
    {
        /**
         * A little bit of key background info... in order for the secret Azure key to carry over
         * from the ArchiveSiteBackend.Api project, the .Tests project must utilize the same
         * <UserSecretsId></UserSecretsId> as described in the projects *.csproj file
         **/
        [Fact]
        public void Has_Azure_Config_Setup()
        {
            Assert.NotNull(AzureCognitiveConfiguration);
            Assert.False(string.IsNullOrEmpty(AzureCognitiveConfiguration.ApiUrl));

            try
            {
                var uri = new Uri(AzureCognitiveConfiguration.ApiUrl);
            } catch(Exception)
            {
                Assert.True(false, $"failed to parse the uri: {AzureCognitiveConfiguration.ApiUrl}");
            }

            Assert.False(string.IsNullOrEmpty(AzureCognitiveConfiguration.ApiKey));
            Assert.True(AzureCognitiveConfiguration.ApiKey.Length == 32);
        }
    }
}
