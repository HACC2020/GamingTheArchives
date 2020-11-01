using ArchiveSite.Data;

namespace ArchiveSiteBackend.Web.Controllers {
    public class DocumentsController : EntityControllerBase<ArchiveDbContext, Document> {
        public DocumentsController(ArchiveDbContext context) : base(context) {
        }
    }
}
