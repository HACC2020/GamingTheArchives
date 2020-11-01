using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class TranscriptionsController : EntityControllerBase<ArchiveDbContext, Transcription> {
        public TranscriptionsController(ArchiveDbContext context) : base(context) {
        }
    }
}
