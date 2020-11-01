using ArchiveSite.Data;

namespace ArchiveSiteBackend.Web.Controllers {
    public class FieldsController  : EntityControllerBase<ArchiveDbContext, Field> {
        public FieldsController(ArchiveDbContext context) : base(context) {
        }

    }
}
