using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class DocumentsController : EntityControllerBase<ArchiveDbContext, Document> {
        public DocumentsController(ArchiveDbContext context) : base(context) {
        }
    }
}
