using System;
using ArchiveSite.Data;

namespace ArchiveSiteBackend.Api.Controllers {
    public class UsersController : EntityControllerBase<ArchiveDbContext, User> {
        public UsersController(ArchiveDbContext context) : base(context) {
        }

        protected override void OnCreating(User entity) {
            base.OnCreating(entity);

            if (this.ModelState.IsValid) {
                entity.LastLogin = entity.SignupDate = DateTimeOffset.UtcNow;
            }
        }
    }
}
