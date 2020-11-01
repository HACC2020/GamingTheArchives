using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ArchiveSite.Data {
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
                optionsBuilder
                    .UseNpgsql(
                        $"Host={configValue.Host};Port={configValue.Port};" +
                        $"Database={configValue.Database};Username={configValue.User};" +
                        $"Password={configValue.Password}")
                    .UseSnakeCaseNamingConvention();
            }
        }

        /// <summary>
        /// <para>
        /// Add unique constraint for User.EmailAddress
        /// </para>
        /// <para>
        /// Add ValueConverters for all DateTime properties to keep times as UTC
        /// </para>
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<User>(e => {
                e.HasIndex(u => u.EmailAddress).IsUnique();
            });

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, DateTimeOffset>(
                v => v.ToUniversalTime(),
                v => v.ToUniversalTime());

            var nullableDateTimeOffsetConverter = new ValueConverter<DateTimeOffset?, DateTimeOffset?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? v.Value.ToUniversalTime() : v);

            foreach (var entityType in builder.Model.GetEntityTypes()) {
                foreach (var property in entityType.GetProperties()) {
                    if (property.ClrType == typeof(DateTime)) {
                        property.SetValueConverter(dateTimeConverter);
                    } else if (property.ClrType == typeof(DateTime?)) {
                        property.SetValueConverter(nullableDateTimeConverter);
                    } else if (property.ClrType == typeof(DateTimeOffset)) {
                        property.SetValueConverter(dateTimeOffsetConverter);
                    } else if (property.ClrType == typeof(DateTimeOffset?)) {
                        property.SetValueConverter(nullableDateTimeOffsetConverter);
                    }
                }
            }
        }

        public static Boolean IsUserError(DbUpdateException exception, out String message) {
            if (exception.InnerException is PostgresException pgException) {
                message = pgException.Detail;
                return true;
            } else {
                message = null;
                return false;
            }
        }
    }
}
