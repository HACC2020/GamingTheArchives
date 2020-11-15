using System;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;

namespace ArchiveSiteBackend.Api.Controllers {
    public class DocumentNotesController : EntityControllerBase<ArchiveDbContext, DocumentNote> {
        private readonly UserContext userContext;

        public DocumentNotesController(ArchiveDbContext context, UserContext userContext) :
            base(context) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        protected override async Task OnCreating(
            DocumentNote entity,
            CancellationToken cancellationToken) {
            await base.OnCreating(entity, cancellationToken);

            if (this.userContext.LoggedInUser == null) {
                throw new InvalidOperationException(
                    "Unable to create a document node from an unauthenticated context"
                );
            }

            if (this.ModelState.IsValid) {
                entity.CreatedTime = DateTimeOffset.UtcNow;;
                entity.AuthorId = this.userContext.LoggedInUser.Id;
            }
        }
    }
}
