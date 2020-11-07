using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ArchiveSite.Data;
using Microsoft.EntityFrameworkCore;

namespace ArchiveSiteBackend.Api.Services {
    public class DbLoginLogger : ILoginLogger {
        private readonly ArchiveDbContext dbContext;

        public DbLoginLogger(ArchiveDbContext dbContext) {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        }
        public async Task LogLogin(ClaimsPrincipal principal) {
            var emailAddress = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (!String.IsNullOrEmpty(emailAddress)) {
                var user = await this.dbContext.Users.SingleOrDefaultAsync(u => u.EmailAddress == emailAddress);
                if (user != null) {
                    user.LastLogin = DateTimeOffset.UtcNow;
                    await this.dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
