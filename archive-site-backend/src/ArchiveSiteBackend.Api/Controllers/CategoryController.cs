using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class CategoryController : EntityControllerBase<ArchiveDbContext, Category> {
        public CategoryController(ArchiveDbContext context) : base(context) {
        }
    }
}
