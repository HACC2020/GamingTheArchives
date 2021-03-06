using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Commands;
using ArchiveSiteBackend.Api.Configuration;
using ArchiveSiteBackend.Api.Middleware;
using ArchiveSiteBackend.Api.Services;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ArchiveSiteBackend.Api {
    public class Startup {
        public static Boolean Insecure { get; set; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            this.ConfigureServices(services, false);
        }

        internal void ConfigureServices(IServiceCollection services, Boolean skipHosting) {
            services.AddLogging(builder =>
                builder.AddConfiguration(this.Configuration.GetSection("Logging")).AddConsole());

            // Add Application Dependencies Here
            services.Configure<ArchiveDbConfiguration>(this.Configuration.GetSection("ArchiveDb"));
            services.AddDbContext<ArchiveDbContext>();

            if (!skipHosting) {
                // Add Hosting specific Dependencies Here

                // Wire up Azure Cognitive config
                var azureConfiguration = new AzureCognitiveConfiguration();
                this.Configuration.GetSection("Azure").Bind(azureConfiguration);
                services.AddSingleton(azureConfiguration);

                services.AddScoped<ICloudOcrService, CognitiveService>();

                services.AddScoped<ILoginLogger, DbLoginLogger>();

                var facebookConfig = new FacebookConfiguration();
                this.Configuration.GetSection("Facebook").Bind(facebookConfig);
                services.AddSingleton(Options.Create(facebookConfig));

                var authenticationBuilder =
                    services
                        .AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
                        .AddCookie(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            options => {
                                options.Events.OnRedirectToLogin = context => {
                                    context.Response.StatusCode = 401;
                                    return Task.CompletedTask;
                                };
                            });

                if (!String.IsNullOrEmpty(facebookConfig.ApplicationId)) {
                    Console.Error.WriteLine($"Enabling Facebook Login with ApplicationId: {facebookConfig.ApplicationId}");
                    authenticationBuilder = authenticationBuilder.AddFacebook(facebookOptions => {
                        facebookOptions.AppId = facebookConfig.ApplicationId;
                        facebookOptions.AppSecret = facebookConfig.Secret;
                        facebookOptions.CallbackPath = "/api/auth/facebook-login";
                        facebookOptions.AccessDeniedPath = "/api/auth/facebook-access-denied";
                        facebookOptions.Fields.Add("email");
                        facebookOptions.Fields.Add("first_name");
                        facebookOptions.Fields.Add("last_name");
                        facebookOptions.Events.OnTicketReceived = context => {
                            var loginLogger = context.HttpContext.RequestServices.GetRequiredService<ILoginLogger>();
                            return loginLogger.LogLogin(context.Principal);
                        };
                    });
                } else {
                    Console.Error.WriteLine("Facebook configuration not found. Facebook login will not be enabled.");
                }

                // Initialized by UserContextMiddleware for each requests
                services.AddScoped<UserContext>();

                var originConfiguration = new OriginPolicyConfiguration();
                this.Configuration.GetSection("OriginPolicy").Bind(originConfiguration);
                services.AddSingleton(Options.Create(originConfiguration));
                if (originConfiguration.HasOrigin()) {
                    services.AddCors(options => {
                        options.AddDefaultPolicy(
                            builder => {
                                builder
                                    .SetIsOriginAllowed(origin =>
                                        IsGlobMatch(originConfiguration.Allow, origin))
                                    .AllowCredentials()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
                    });
                }

                // Asp.Net MVC Dependencies
                services
                    .AddControllers(options => {
                        var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();

                        if (!String.IsNullOrEmpty(facebookConfig.ApplicationId)) {
                            options.Filters.Add(new AuthorizeFilter(policy));
                        }
                    })
                    .AddNewtonsoftJson(options => {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver {
                            // Use TitleCase naming everywhere so that we're consistent with OData endpoints
                            NamingStrategy = new DefaultNamingStrategy()
                        };
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });

                services.AddMvc();
                services.AddOData();
            } // End of Hosting Specific Dependencies

            // Register Commands
            services.AddScoped<InitializeCommand>();
        }

        private static Boolean IsGlobMatch(String pattern, String origin) {
            var regex = new Regex(Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", "."));
            return regex.IsMatch(origin);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            var originPolicy = app.ApplicationServices.GetRequiredService<IOptions<OriginPolicyConfiguration>>();
            if (originPolicy.Value.HasOrigin()) {
                app.UseCors();
            }

            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();

            app.UseMiddleware<UserContextMiddleware>();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Expand().Filter().OrderBy().Count().MaxTop(Int32.MaxValue);
                endpoints.SetUrlKeyDelimiter(ODataUrlKeyDelimiter.Parentheses);
                endpoints.MapODataRoute("odata", "api/odata", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel() {
            var odataBuilder = new ODataConventionModelBuilder();

            var users = odataBuilder.EntitySet<User>("Users");
            var projects = odataBuilder.EntitySet<Project>("Projects");
            odataBuilder.EntitySet<Category>("Categories");
            odataBuilder.EntitySet<Document>("Documents");
            odataBuilder.EntitySet<DocumentAction>("DocumentActions");
            odataBuilder.EntitySet<DocumentNote>("DocumentNotes");
            odataBuilder.EntitySet<Field>("Fields");
            odataBuilder.EntitySet<Transcription>("Transcriptions");
            var activities = odataBuilder.EntitySet<Activity>("Activities");

            var meFunction = users.EntityType.Collection.Function("Me");
            meFunction.ReturnsFromEntitySet<User>("Users");

            var userActivities = users.EntityType.Function("Activities");
            userActivities.ReturnsFromEntitySet<Activity>("Activities");

            var saveProfileAction = users.EntityType.Collection.Action("SaveProfile");
            saveProfileAction.EntityParameter<User>("profile");
            saveProfileAction.ReturnsFromEntitySet<User>("Users");

            var currentUserActivities = activities.EntityType.Collection.Function("CurrentUser");
            currentUserActivities.ReturnsFromEntitySet<Activity>("Activities");

            var nextDocumentFunction = projects.EntityType.Function("NextDocument");
            nextDocumentFunction.ReturnsFromEntitySet<Document>("documents");

            return odataBuilder.GetEdmModel();
        }
    }
}
