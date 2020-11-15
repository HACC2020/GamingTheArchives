using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class CategoriesController : EntityControllerBase<ArchiveDbContext, Category> {
        public CategoriesController(ArchiveDbContext context) : base(context) {
        }
    }
}
