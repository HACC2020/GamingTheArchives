using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class ProjectsController : EntityControllerBase<ArchiveDbContext, Project> {
        public ProjectsController(ArchiveDbContext context) : base(context) {
        }
    }
}
