using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class FieldsController  : EntityControllerBase<ArchiveDbContext, Field> {
        public FieldsController(ArchiveDbContext context) : base(context) {
        }

    }
}
