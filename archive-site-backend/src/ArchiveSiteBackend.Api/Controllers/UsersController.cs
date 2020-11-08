using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ArchiveSiteBackend.Api.Controllers {
    public class UsersController : EntityControllerBase<ArchiveDbContext, User> {
        private readonly ILogger<UsersController> logger;

        public UsersController(
            ArchiveDbContext context,
            ILogger<UsersController> logger) : base(context) {

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [AllowAnonymous]
        [HttpGet("ident")]
        public IActionResult Ident() {
            return Ok(
                this.User?.Identity?.IsAuthenticated == true ?
                    new { Email = this.User.FindFirst(ClaimTypes.Email)?.Value } :
                    new Object()
            );
        }

        public async Task<IActionResult> Me() {
            if (this.User?.Identity?.IsAuthenticated != true) {
                // This shouldn't actually get hit
                this.logger.LogWarning("An unauthenticated request for UsersController.Me occurred.");
                return Unauthorized();
            }

            var email = this.User.FindFirst(ClaimTypes.Email).Value;
            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.EmailAddress == email);

            if (user == null) {
                return NotFound();
            } else {
                return Ok(user);
            }
        }

        /// <summary>
        /// Create or update the current user's profile.
        /// </summary>
        /// <param name="profile">The current user's profile.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created or updated profile.</returns>
        public async Task<IActionResult> SaveProfile([FromBody] User profile, CancellationToken cancellationToken) {
            if (this.User?.Identity?.IsAuthenticated != true) {
                // This shouldn't actually get hit
                this.logger.LogWarning("An unauthenticated request for UsersController.SaveProfile occurred.");
                return Unauthorized();
            }

            var email = this.User.FindFirst(ClaimTypes.Email).Value;
            var user = await this.Context.Users.SingleOrDefaultAsync(u => u.EmailAddress == email, cancellationToken: cancellationToken);

            if (!String.Equals(email, profile.EmailAddress, StringComparison.OrdinalIgnoreCase)) {
                return BadRequest("Your email address cannot be changed.");
            }

            String operation;
            Func<User, IActionResult> successResult;
            if (user == null) {
                // New SignUp
                profile.LastLogin = profile.SignUpDate = DateTimeOffset.UtcNow;
                user = (await this.Context.Users.AddAsync(profile, cancellationToken)).Entity;
                operation = "create";
                successResult = this.Created;
            } else {
                // Profile Update
                profile.CopyTo(user);
                operation = "update";
                successResult = this.Ok;
            }

            return await this.TrySaveChanges(
                user,
                successResult,
                operation,
                cancellationToken
            );
        }

        protected override void OnCreating(User entity) {
            base.OnCreating(entity);

            if (this.ModelState.IsValid) {
                entity.LastLogin = entity.SignUpDate = DateTimeOffset.UtcNow;
            }
        }
    }
}
