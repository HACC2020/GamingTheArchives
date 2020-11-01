using System;
using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class DocumentActionsController : EntityControllerBase<ArchiveDbContext, DocumentAction> {
        public DocumentActionsController(ArchiveDbContext context) : base(context) {
        }

        protected override void OnCreating(DocumentAction entity) {
            base.OnCreating(entity);

            if (this.ModelState.IsValid) {
                entity.ActionTime = DateTimeOffset.UtcNow;
            }
        }
    }
}
