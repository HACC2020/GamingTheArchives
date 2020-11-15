using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ArchiveSiteBackend.Api.Controllers {
    public class TranscriptionsController : EntityControllerBase<ArchiveDbContext, Transcription> {
        private readonly UserContext userContext;

        public TranscriptionsController(ArchiveDbContext dbContext, UserContext userContext) :
            base(dbContext) {

            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        protected override async Task OnCreated(
            Transcription createdEntity,
            CancellationToken cancellationToken) {

            await base.OnCreated(createdEntity, cancellationToken);
            if (createdEntity.IsSubmitted) {
                await this.RecordTranscriptionCreatedActivity(createdEntity, true, cancellationToken);
            }
        }

        protected override async Task OnUpdating(
            Int64 key,
            Transcription existing,
            Transcription update,
            CancellationToken cancellationToken) {

            await base.OnUpdating(key, existing, update, cancellationToken);

            if (!existing.IsSubmitted && update.IsSubmitted) {
                await RecordTranscriptionCreatedActivity(update, false, cancellationToken);
            }
        }

        private async Task RecordTranscriptionCreatedActivity(
            Transcription transcription,
            Boolean saveChanges,
            CancellationToken cancellationToken) {

            if (this.userContext.LoggedInUser == null) {
                throw new InvalidOperationException("Missing authorization context.");
            }

            var project = await
                this.DbContext.Documents
                    .Where(d => d.Id == transcription.DocumentId)
                    .Join(this.DbContext.Projects,
                        d => d.ProjectId,
                        p => p.Id,
                        (_, p) => p)
                    .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            await this.DbContext.Activities.AddAsync(
                new Activity {
                    Type = ActivityType.TranscriptionSubmitted,
                    Message = $"{{{{DisplayName}}}} submitted a transcription for the project {project?.Name ?? "Untitled"}",
                    EntityType = nameof(Transcription),
                    EntityId = transcription.Id,
                    UserId = this.userContext.LoggedInUser.Id,
                    CreatedTime = DateTimeOffset.UtcNow
                },
                cancellationToken
            );

            if (saveChanges) {
                await this.DbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
