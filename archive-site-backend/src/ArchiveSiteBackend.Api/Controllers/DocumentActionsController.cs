using System;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class DocumentActionsController : EntityControllerBase<ArchiveDbContext, DocumentAction> {
        public DocumentActionsController(ArchiveDbContext context) : base(context) {
        }

        protected override async Task OnCreating(DocumentAction entity, CancellationToken cancellationToken) {
            await base.OnCreating(entity, cancellationToken);

            if (this.ModelState.IsValid) {
                entity.ActionTime = DateTimeOffset.UtcNow;
            }
        }
    }
}
