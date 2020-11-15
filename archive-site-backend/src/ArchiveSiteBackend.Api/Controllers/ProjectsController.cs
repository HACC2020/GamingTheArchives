using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiveSiteBackend.Api.Controllers {
    public class ProjectsController : EntityControllerBase<ArchiveDbContext, Project> {
        private readonly UserContext userContext;

        public ProjectsController(ArchiveDbContext context, UserContext userContext) :
            base(context) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task<IActionResult> NextDocument([FromODataUri] Int64 key, CancellationToken cancellationToken) {
            if (this.userContext.LoggedInUser == null) {
                // Should not happen
                return Unauthorized();
            }

            // Get the next document in the sequence that meets the following requirements:
            //  * The document status is PendingTranscription
            //  * The current user has not already transcribed it
            //  * The current user has not already skipped it
            //  * Another user has not begun transcription within the last 30 minutes

            var nextDocument = await
                this.DbContext.Documents
                    .Where(d => d.ProjectId == key && d.Status == DocumentStatus.PendingTranscription)
                    .GroupJoin(
                        this.DbContext.Transcriptions.Where(t => t.UserId == this.userContext.LoggedInUser.Id),
                        d => d.Id,
                        t => t.Id,
                        (d, t) => new { Document = d, Transcription = t.SingleOrDefault() })
                    .Where(r => r.Transcription == null)
                    .GroupJoin(
                        this.DbContext.DocumentActions.Where(da =>
                            da.UserId == this.userContext.LoggedInUser.Id && da.Type == DocumentActionType.SkipTranscription ||
                            da.UserId != this.userContext.LoggedInUser.Id && da.Type == DocumentActionType.BeginTranscription &&
                            da.ActionTime > DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(30))),
                        r => r.Document.Id,
                        da => da.DocumentId,
                        (r, da) => new { r.Document, Actions = da })
                    .SelectMany(
                        r => r.Actions,
                        (r, action) => new { r.Document, Action = action })
                    .Where(r => r.Action == null)
                    .Select(r => r.Document)
                    .OrderBy(d => d.FileName)
                    .FirstOrDefaultAsync(cancellationToken);

            if (nextDocument != null) {
                return Ok(nextDocument);
            } else {
                return NotFound();
            }
        }

        protected override async Task OnCreated(
            Project createdEntity,
            CancellationToken cancellationToken) {

            await base.OnCreated(createdEntity, cancellationToken);
            if (createdEntity.Active) {
                await this.RecordProjectCreatedActivity(createdEntity, true, cancellationToken);
            }
        }

        protected override async Task OnUpdating(
            Int64 key,
            Project existing,
            Project update,
            CancellationToken cancellationToken) {

            await base.OnUpdating(key, existing, update, cancellationToken);

            if (!existing.Active && update.Active) {
                await this.RecordProjectCreatedActivity(update, false, cancellationToken);
            }
        }

        private async Task RecordProjectCreatedActivity(Project project, Boolean saveChanges, CancellationToken cancellationToken) {
            await this.DbContext.Activities.AddAsync(
                new Activity {
                    ActivityType = ActivityType.ProjectCreated,
                    Message = $"{this.userContext.LoggedInUser.DisplayName} has created a new project: {project.Name}!",
                    EntityType = nameof(Project),
                    EntityId = project.Id,
                    UserId = this.userContext.LoggedInUser.Id,
                    ActivityTime = DateTimeOffset.UtcNow
                },
                cancellationToken
            );

            if (saveChanges) {
                await this.DbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
