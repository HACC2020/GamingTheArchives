using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using Microsoft.AspNet.OData;
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
                return this.Unauthorized();
            }

            var email = this.User.FindFirst(ClaimTypes.Email).Value;
            var user = await this.DbContext.Users.SingleOrDefaultAsync(u => u.EmailAddress == email);

            if (user == null) {
                return this.NotFound();
            } else {
                return this.Ok(user);
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
            var user = await this.DbContext.Users.SingleOrDefaultAsync(u => u.EmailAddress == email, cancellationToken: cancellationToken);

            if (!String.Equals(email, profile.EmailAddress, StringComparison.OrdinalIgnoreCase)) {
                return BadRequest("Your email address cannot be changed.");
            }

            String operation;
            Func<User, Task<IActionResult>> successResult;
            if (user == null) {
                // New SignUp
                await this.OnCreating(profile, cancellationToken);
                var userEntity = (await this.DbContext.Users.AddAsync(profile, cancellationToken));
                user = userEntity.Entity;
                operation = "create";
                successResult = async createdUser => {
                    await this.OnCreated(createdUser, cancellationToken);
                    return (IActionResult)Created(createdUser);
                };
            } else {
                if (profile.Id == 0) {
                    profile.Id = user.Id;
                }

                await this.OnUpdating(user.Id, user, profile, cancellationToken);
                // Profile Update
                profile.CopyTo(user);
                operation = "update";
                successResult = async updatedUser => {
                    await this.OnUpdated(updatedUser, cancellationToken);
                    return Ok(updatedUser);
                };
            }

            return await this.TrySaveChanges(
                user,
                successResult,
                operation,
                cancellationToken
            );
        }


        [EnableQuery]
        [AllowAnonymous]
        public virtual IQueryable<Activity> Activities([FromODataUri] Int64 key) {
            return this.DbContext.Activities.Where(a => a.UserId == key);
        }

        protected override async Task OnCreating(User entity, CancellationToken cancellationToken) {
            await base.OnCreating(entity, cancellationToken);

            if (this.ModelState.IsValid) {
                entity.LastLogin = entity.SignUpDate = DateTimeOffset.UtcNow;
            }
        }

        protected override async Task OnCreated(User user, CancellationToken cancellationToken) {
            await base.OnCreated(user, cancellationToken);
            await this.CreateSignUpActivity(user, cancellationToken);
        }

        private async Task CreateSignUpActivity(User user, CancellationToken cancellationToken) {
            await this.DbContext.Activities.AddAsync(
                new Activity {
                    UserId = user.Id,
                    Message = $"Aloha e {user.DisplayName}. Thank you for signing up!",
                    ActivityType = ActivityType.UserSignup,
                    ActivityTime = DateTimeOffset.UtcNow
                },
                cancellationToken
            );

            await this.DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
