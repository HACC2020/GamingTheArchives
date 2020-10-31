using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ArchiveSiteBackend.Data {
    public class ArchiveDbContext : DbContext {
        private readonly IOptions<ArchiveDbConfiguration> config;

        public DbSet<User> Users => this.Set<User>();
        public DbSet<Project> Projects => this.Set<Project>();
        public DbSet<Document> Documents => this.Set<Document>();
        public DbSet<DocumentAction> DocumentActions => this.Set<DocumentAction>();
        public DbSet<DocumentNote> DocumentNotes => this.Set<DocumentNote>();
        public DbSet<Field> Fields => this.Set<Field>();
        public DbSet<Transcription> Transcriptions => this.Set<Transcription>();

        public ArchiveDbContext(IOptions<ArchiveDbConfiguration> config) {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                var configValue = this.config.Value;
                optionsBuilder.UseNpgsql(
                    $"Host={configValue.Host};Port={configValue.Port};Database={configValue.Database};Username={configValue.User};Password={configValue.Password}"
                );
            }
        }
    }
}
