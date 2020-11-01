using ArchiveSite.Data;

namespace ArchiveSiteBackend.Web.Controllers {
    public class TranscriptionsController : EntityControllerBase<ArchiveDbContext, Transcription> {
        public TranscriptionsController(ArchiveDbContext context) : base(context) {
        }
    }
}
