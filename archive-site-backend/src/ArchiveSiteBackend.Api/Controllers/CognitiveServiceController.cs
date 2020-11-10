using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace ArchiveSiteBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CognitiveServiceController : ControllerBase
    {
        private ILogger<CognitiveServiceController> Logger;
        private ArchiveDbContext ArchiveDbContext;
        private ICloudOcrService CognitiveService;

        public CognitiveServiceController(ILogger<CognitiveServiceController> logger, ArchiveDbContext archiveDbContext,
            ICloudOcrService cognitiveService)
        {
            Logger = logger;
            ArchiveDbContext = archiveDbContext;
            CognitiveService = cognitiveService;
        }

        /// <summary>
        /// This method accepts a documentId and submits the document image to the Azure Cognitive
        /// Service for OCR
        /// </summary>
        /// <param name="documentId">refers to the document id in the table documents column id</param>
        /// <returns></returns>
        [Obsolete("This method will change...")]
        public async Task<IActionResult> Post([FromBody] Int64 documentId)
        {
            var document = ArchiveDbContext.Documents.Find(documentId);
            if (document == null)
            {
                return NotFound();
            }

            var httpClient = new HttpClient();

            try
            {
                var url = new Uri(document.DocumentImageUrl);
                var httpStream = await httpClient.GetStreamAsync(url);

                var documentTexts = await CognitiveService.ReadImage(httpStream);

                return Ok(documentTexts);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"failed to read image for documentId: {documentId}");
                return NotFound();
            }
        }
    }
}
