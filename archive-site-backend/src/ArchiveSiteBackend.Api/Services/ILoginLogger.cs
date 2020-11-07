using System.Security.Claims;
using System.Threading.Tasks;

namespace ArchiveSiteBackend.Api.Services {
    public interface ILoginLogger {
        public Task LogLogin(ClaimsPrincipal principal);
    }
}
