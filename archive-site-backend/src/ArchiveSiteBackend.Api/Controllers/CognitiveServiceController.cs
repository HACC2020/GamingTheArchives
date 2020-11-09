using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;
using Microsoft.Extensions.Logging;

namespace ArchiveSiteBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CognitiveServiceController : ControllerBase
    {
        private ILogger<CognitiveServiceController> Logger;
        private ArchiveDbContext ArchiveDbContext;
        private CognitiveService CognitiveService;

        public CognitiveServiceController(ILogger<CognitiveServiceController> logger, ArchiveDbContext archiveDbContext,
            CognitiveService cognitiveService)
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
        public async Task<IActionResult> Post(Int64 documentId)
        {
            var document = ArchiveDbContext.Documents.Find(documentId);
            if (document == null)
            {
                return NotFound();
            }

            // TODO complete the implementation

            var documentTexts = await CognitiveService.ReadImage(System.IO.File.OpenRead(document.FileName));


            return Ok(documentTexts);
        }
    }
}
