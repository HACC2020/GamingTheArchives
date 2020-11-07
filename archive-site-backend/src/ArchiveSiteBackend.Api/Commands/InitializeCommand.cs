using System;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.CommandLineOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchiveSiteBackend.Api.Commands {
    public class InitializeCommand {
        private readonly ArchiveDbConfiguration dbConfiguration;
        private readonly ArchiveDbContext context;
        private readonly ILogger<InitializeCommand> logger;

        public InitializeCommand(
            IOptions<ArchiveDbConfiguration> dbConfiguration,
            ArchiveDbContext context,
            ILogger<InitializeCommand> logger) {

            this.dbConfiguration = (dbConfiguration ?? throw new ArgumentNullException(nameof(dbConfiguration))).Value;
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task InvokeAsync(InitializeOptions options, CancellationToken cancellationToken) {
            if (options.DropDatabase) {
                this.logger.LogInformation(
                    "Dropping Database {DatabaseName} (Environment = {Environment}, Provider = {Provider})",
                    this.dbConfiguration.Database,
                    options.Environment,
                    this.context.Database.ProviderName
                );

                await context.Database.EnsureDeletedAsync(cancellationToken);
            }

            var result = await context.Database.EnsureCreatedAsync(cancellationToken);

            this.logger.LogInformation(
                result ?
                    "Created Database {DatabaseName} (Environment = {Environment}, Provider = {Provider})" :
                    "The Database {DatabaseName} Already Exists (Environment = {Environment}, Provider = {Provider})",
                this.dbConfiguration.Database,
                options.Environment,
                this.context.Database.ProviderName
            );
        }
    }
}
