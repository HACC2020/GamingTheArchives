using ArchiveSiteBackend.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArchiveSiteBackend.Api.Tests.Controllers
{
    public class CognitiveServiceTests : UnitTestBase
    {
        [Fact]
        public async Task ReadImage_Test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<CognitiveService>>();
            var cognitiveService = new CognitiveService(mockLogger.Object, AzureCognitiveConfiguration);

            // Act
            var documentTexts = await cognitiveService.ReadImage(Path.Combine("Samples", "GlassNegatives_00003.pdf"));

            // Assert
            Assert.NotNull(documentTexts);
            Assert.True(documentTexts.Count > 0);
        }
    }
}
