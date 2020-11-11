using ArchiveSite.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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

        /// <summary>
        /// The image must be returned from the host; Chrome CORS canvas security does not allow rendering
        /// of images across domains.
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        [HttpGet("{documentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Int64 documentId)
        {
            var document = await ArchiveDbContext.Documents.FindAsync(documentId);

            if (null == document || string.IsNullOrEmpty(document.DocumentImageUrl))
            {
                return NotFound();
            }

            var httpClient = new HttpClient();

            try
            {
                var url = new Uri(document.DocumentImageUrl);
                var httpStream = await httpClient.GetStreamAsync(url);

                Response.Headers.Add("Access-Control-Allow-Origin", "Anonymous");
                return File(httpStream, MediaTypeNames.Image.Jpeg);
            } catch(Exception)
            {
                Logger.LogError($"failed to parse the documentId {documentId} url of {document.DocumentImageUrl}");
                return NotFound();
            }
        }
    }
}
