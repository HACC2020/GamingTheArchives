using ArchiveSite.Data;

namespace ArchiveSiteBackend.Web.Controllers {
    public class ProjectsController : EntityControllerBase<ArchiveDbContext, Project> {
        public ProjectsController(ArchiveDbContext context) : base(context) {
        }
    }
}
