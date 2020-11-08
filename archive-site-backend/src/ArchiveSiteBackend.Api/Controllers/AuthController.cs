using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArchiveSiteBackend.Api.Configuration;
using ArchiveSiteBackend.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ArchiveSiteBackend.Api.Controllers {
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : Controller {
        private readonly IOptions<FacebookConfiguration> facebookConfiguration;

        public AuthController(IOptions<FacebookConfiguration> facebookConfiguration) {
            this.facebookConfiguration = facebookConfiguration ?? throw new ArgumentNullException(nameof(facebookConfiguration));
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] String returnUrl) {
            if (this?.User?.Identity.IsAuthenticated == true) {
                return this.Redirect(returnUrl ?? "/");
            }

            return this.View(new LoginModel {
                ReturnUrl = returnUrl ?? "/",
                Insecure = Startup.Insecure
            });
        }

        [HttpPost("insecure-login")]
        public async Task<IActionResult> InsecureLogin(String email, String returnUrl) {
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new[] {
                    new ClaimsIdentity(
                        new[] {
                            new Claim(ClaimTypes.Email, email)
                        },
                        "Insecure"
                    )
                })
            );

            return String.IsNullOrWhiteSpace(returnUrl) ?
                (IActionResult)RedirectToAction("Test") :
                Redirect(returnUrl);
        }

        [HttpGet("login-with-facebook")]
        public async Task LoginWithFacebook([FromQuery] String returnUrl) {
            if (Startup.Insecure) {
                HttpContext.Response.Redirect(Url.Action("Login", new { returnUrl }));
                return;
            }

            await HttpContext.ChallengeAsync(
                FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties {
                    RedirectUri = returnUrl ?? "/",
                }
            );
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout([FromQuery] String returnUrl) {
            await HttpContext.SignOutAsync(
                new AuthenticationProperties {
                    RedirectUri = returnUrl ?? "/"
                }
            );

            return Redirect(returnUrl);
        }

        [HttpGet("test")]
        public IActionResult Test() {
            return Json(new {
                this.User.Identity.Name,
                this.User.Identity.AuthenticationType,
                this.User.Identity.IsAuthenticated,
                Claims = this.User.Claims.Select(c => new {
                    c.Issuer,
                    c.OriginalIssuer,
                    c.Type,
                    c.Value,
                    c.ValueType
                })
            });
        }
    }
}
