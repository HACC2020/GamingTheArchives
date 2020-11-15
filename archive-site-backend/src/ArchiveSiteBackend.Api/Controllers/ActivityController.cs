using System;
using System.Linq;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;

namespace ArchiveSiteBackend.Api.Controllers {
    public class ActivityController : EntityControllerBase<ArchiveDbContext, Activity> {
        private readonly UserContext userContext;

        public ActivityController(ArchiveDbContext context, UserContext userContext) :
            base(context) {
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        [EnableQuery]
        [AllowAnonymous]
        public IQueryable<Activity> CurrentUser() {
            if (this.userContext.LoggedInUser != null) {
                return this.DbContext.Activities.Where(
                    a => a.UserId == this.userContext.LoggedInUser.Id
                );
            } else {
                return Enumerable.Empty<Activity>().AsQueryable();
            }
        }
    }
}
