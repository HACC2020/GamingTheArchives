using System;
using System.Text.Json;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Web.Commands;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace ArchiveSiteBackend.Web {
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddMvc();
            services.AddOData();

            // Register Commands
            services.AddScoped<InitializeCommand>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().Filter().OrderBy().Count();
                endpoints.SetUrlKeyDelimiter(ODataUrlKeyDelimiter.Parentheses);
                endpoints.MapODataRoute("odata", "api", GetEdmModel());
            });
        }

        private static IEdmModel GetEdmModel()
        {
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
