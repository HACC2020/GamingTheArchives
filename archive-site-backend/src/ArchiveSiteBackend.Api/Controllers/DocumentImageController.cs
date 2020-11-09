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

            if (null == document)
            {
                return NotFound();
            }

            try
            {
                var imageFile = System.IO.File.OpenRead(document.FileName);
                return File(imageFile, MediaTypeNames.Image.Jpeg);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"failed to open filename '{document.FileName}' from documentId '{document.Id}");
                return NotFound();
            }
        }
    }
}
