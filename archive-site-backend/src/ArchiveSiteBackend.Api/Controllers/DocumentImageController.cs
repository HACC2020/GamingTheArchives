using ArchiveSite.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentImageController : ControllerBase
    {
        private ILogger<DocumentImageController> Logger;
        private ArchiveDbContext ArchiveDbContext;

        public DocumentImageController(ILogger<DocumentImageController> logger, ArchiveDbContext archiveDbContext)
        {
            Logger = logger;
            ArchiveDbContext = archiveDbContext;
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> Get(Int64 documentId)
        {
            var document = await ArchiveDbContext.Documents.FindAsync(documentId);

            if (null == document || string.IsNullOrEmpty(document.DocumentImageUrl))
            {
                return NotFound();
            }

            try
            {
                var url = new Uri(document.DocumentImageUrl);
                return Redirect(document.DocumentImageUrl);
            } catch(Exception)
            {
                Logger.LogError($"failed to parse the documentId {documentId} url of {document.DocumentImageUrl}");
                return NotFound();
            }
        }
    }
}
