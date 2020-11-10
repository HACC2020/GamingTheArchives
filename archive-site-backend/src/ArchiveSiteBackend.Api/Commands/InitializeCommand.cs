using System;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using ArchiveSiteBackend.Api.CommandLineOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchiveSiteBackend.Api.Commands
{
    public class InitializeCommand
    {
        private readonly ArchiveDbConfiguration dbConfiguration;
        private readonly ArchiveDbContext context;
        private readonly ILogger<InitializeCommand> logger;

        public InitializeCommand(
            IOptions<ArchiveDbConfiguration> dbConfiguration,
            ArchiveDbContext context,
            ILogger<InitializeCommand> logger)
        {

            this.dbConfiguration = (dbConfiguration ?? throw new ArgumentNullException(nameof(dbConfiguration))).Value;
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task InvokeAsync(InitializeOptions options, CancellationToken cancellationToken)
        {
            if (options.DropDatabase)
            {
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

            if (options.SeedDatabase)
            {
                this.logger.LogInformation(
                    result ?
                        "Attempting To Seed Database {DatabaseName} (Environment = {Environment}, Provider = {Provider})" :
                        "Unable To Seed Existing Database {DatabaseName}. Seed Again Using Drop. (Environment = {Environment}, Provider = {Provider})",
                    this.dbConfiguration.Database,
                    options.Environment,
                    this.context.Database.ProviderName
                );

                if(!result)
                {
                    return;
                }

                context.Projects.Add(new Project() { Active = true, Description = "This demo project uses the sample images uploaded by Paul and hosted by Google", Name = "Demo Project 1"});
                context.Projects.Add(new Project() { Active = true, Description = "This demo project uses the sample images uploaded by Paul and hosted by Google", Name = "Demo Project 2"});
                context.Projects.Add(new Project() { Active = false, Description = "description 3", Name = "Inactive Sample Project 3"});

                context.Documents.Add(new Document() {ProjectId = 1, FileName = "ChineseArrivals_1878_01218.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01218.png"});
                context.Documents.Add(new Document() {ProjectId = 1, FileName = "ChineseArrivals_1878_01289.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01289.png"});
                context.Documents.Add(new Document() {ProjectId = 1, FileName = "ChineseArrivals_1878_01271.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01271.png"});

                context.Documents.Add(new Document() {ProjectId = 2, FileName = "ChineseArrivals_1878_01238.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01238.png"});
                context.Documents.Add(new Document() {ProjectId = 2, FileName = "ChineseArrivals_1878_01218.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01218.png"});
                context.Documents.Add(new Document() {ProjectId = 2, FileName = "ChineseArrivals_1878_01289.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01289.png"});

                context.Documents.Add(new Document() {ProjectId = 3, FileName = "ChineseArrivals_1878_01218.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01218.png"});
                context.Documents.Add(new Document() {ProjectId = 3, FileName = "ChineseArrivals_1878_01289.png", Status = DocumentStatus.PendingTranscription, DocumentImageUrl = "https://storage.googleapis.com/app-hiscribe-images/projects/1/ChineseArrivals_1878_01289.png"});

                var saveChanges = await context.SaveChangesAsync();

                this.logger.LogInformation(
                    saveChanges > 0 ?
                        "Successfully Seeded Database {DatabaseName} (Environment = {Environment}, Provider = {Provider})" :
                        "Failed To Seed The Database {DatabaseName} (Environment = {Environment}, Provider = {Provider})",
                    this.dbConfiguration.Database,
                    options.Environment,
                    this.context.Database.ProviderName
                );
            }
        }
    }
}
