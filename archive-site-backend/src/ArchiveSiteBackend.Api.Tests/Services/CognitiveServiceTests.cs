using ArchiveSiteBackend.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ArchiveSiteBackend.Api.Tests.Services
{
    public class CognitiveServiceTests : UnitTestBase
    {
        [Fact]
        public async Task ReadImage_Test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CognitiveService>>();
            var cognitiveService = new CognitiveService(mockLogger.Object, AzureCognitiveConfiguration);
            var fileStream = File.OpenRead("Samples\\GlassNegatives_00003.pdf");

            // Act
            var documentTexts = await cognitiveService.ReadImage(fileStream);

            // Assert
            Assert.NotNull(documentTexts);
            Assert.True(documentTexts.Count > 0);
        }
    }
}
