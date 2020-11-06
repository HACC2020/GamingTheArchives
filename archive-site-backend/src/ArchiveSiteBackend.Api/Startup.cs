using System;
using System.Linq;
using System.Text.RegularExpressions;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.Commands;
using ArchiveSiteBackend.Api.Configuration;
using ArchiveSiteBackend.Api.Services;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Newtonsoft.Json.Serialization;

namespace ArchiveSiteBackend.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddLogging(builder =>
                builder.AddConfiguration(this.Configuration.GetSection("Logging")).AddConsole());
            services.Configure<ArchiveDbConfiguration>(this.Configuration.GetSection("ArchiveDb"));
            services.AddDbContext<ArchiveDbContext>();
            services.AddControllers()
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver {
                        // Use TitleCase naming everywhere so that we're consistent with OData endpoints
                        NamingStrategy = new DefaultNamingStrategy()
                    };
                });
            services.AddMvc();
            services.AddOData();

            // wire up azure cognitive config
            var azureConfiguration = new AzureCognitiveConfiguration();
            this.Configuration.GetSection("Azure").Bind(azureConfiguration);
            services.AddSingleton(azureConfiguration);

            // allow cognitive service to be injectable
            services.AddScoped<CognitiveService>();

            var originConfiguration = new OriginPolicyConfiguration();
            this.Configuration.GetSection("OriginPolicy").Bind(originConfiguration);
            services.AddSingleton(Microsoft.Extensions.Options.Options.Create(originConfiguration));
            if (originConfiguration.HasOrigin()) {
                services.AddCors(options => {
                    options.AddDefaultPolicy(
                        builder => {
                            builder
                                .SetIsOriginAllowed(origin =>
                                    IsGlobMatch(originConfiguration.Allow, origin)
                                    /* originConfiguration.AllowOrigin.Any(pattern => IsGlobMatch(pattern, origin)) */)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                });
            }

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

            // app.UseHttpsRedirection();

            var originPolicy = app.ApplicationServices.GetRequiredService<IOptions<OriginPolicyConfiguration>>();
            if (originPolicy.Value.HasOrigin()) {
                app.UseCors();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count();
                endpoints.SetUrlKeyDelimiter(ODataUrlKeyDelimiter.Parentheses);
                endpoints.MapODataRoute("odata", "api/odata", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel() {
            var odataBuilder = new ODataConventionModelBuilder();

            odataBuilder.EntitySet<User>("Users");
            odataBuilder.EntitySet<Project>("Projects");
            odataBuilder.EntitySet<Document>("Documents");
            odataBuilder.EntitySet<DocumentAction>("DocumentActions");
            odataBuilder.EntitySet<DocumentNote>("DocumentNotes");
            odataBuilder.EntitySet<Field>("Fields");
            odataBuilder.EntitySet<Transcription>("Transcriptions");

            return odataBuilder.GetEdmModel();
        }
    }
}
