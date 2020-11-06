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

        [Obsolete("This method will change...")]
        public async Task<IActionResult> Post(string documentId)
        {
            var document = ArchiveDbContext.Documents.Find(documentId);
            if (document == null)
            {
                return NotFound();
            }

            // TODO complete the implementation

            var documentTexts = await CognitiveService.ReadImage(document.FileName);


            return Ok(documentTexts);
        }
    }
}
