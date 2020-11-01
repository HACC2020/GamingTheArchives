using System;
using ArchiveSite.Data;

namespace ArchiveSiteBackend.Web.Controllers {
    public class DocumentNotesController : EntityControllerBase<ArchiveDbContext, DocumentNote> {
        public DocumentNotesController(ArchiveDbContext context) : base(context) {
        }

        protected override void OnCreating(DocumentNote entity) {
            base.OnCreating(entity);

            if (this.ModelState.IsValid) {
                entity.CreatedTime = DateTimeOffset.UtcNow;;
            }
        }
    }
}
