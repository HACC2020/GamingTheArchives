using System.Security.Claims;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ArchiveSiteBackend.Api.Middleware {
    public class UserContextMiddleware : RequestMiddlewareBase<ArchiveDbContext, UserContext> {
        public UserContextMiddleware(RequestDelegate nextRequestDelegate) : base(nextRequestDelegate) {
        }

        protected override async Task OnInvoke(
            HttpContext context,
            ArchiveDbContext dbContext,
            UserContext userContext) {
            var emailClaim = context.User?.FindFirst(ClaimTypes.Email);
            if (emailClaim != null) {
                userContext.LoggedInUser = await
                    dbContext.Users
                        .SingleOrDefaultAsync(u =>
                            u.EmailAddress.ToUpper() == emailClaim.Value.ToUpper()
                        );
            } else {
                userContext.LoggedInUser = null;
            }
        }
    }
}
