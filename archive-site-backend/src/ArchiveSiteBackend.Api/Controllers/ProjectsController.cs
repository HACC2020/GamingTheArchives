using System;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;

namespace ArchiveSiteBackend.Api.Controllers {
    public class ProjectsController : EntityControllerBase<ArchiveDbContext, Project> {
        private readonly UserContext userContext;

        public ProjectsController(ArchiveDbContext context, UserContext userContext) :
            base(context) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
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
