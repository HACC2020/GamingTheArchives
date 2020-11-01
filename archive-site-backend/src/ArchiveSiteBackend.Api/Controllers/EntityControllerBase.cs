using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiveSiteBackend.Api.Controllers {
    [ApiController]
    [Route("api/odata/[controller]")]
    public abstract class EntityControllerBase<TContext, TEntity> : ODataController
        where TContext : DbContext
        where TEntity : EntityBase<TEntity> {
        protected TContext Context { get; }

        protected virtual JsonSerializerOptions SerializerOptions { get; }

        protected EntityControllerBase(TContext context) {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));

            var serializerOptions = this.SerializerOptions = new JsonSerializerOptions();
            serializerOptions.Converters.Insert(0, new JsonStringEnumConverter());
        }

        [EnableQuery]
        public virtual IQueryable<TEntity> Get() {
            return this.Context.Set<TEntity>();
        }

        [EnableQuery]
        public virtual async Task<IActionResult> Get([FromODataUri] Int64 key, CancellationToken cancellationToken) {
            var dbSet = this.Context.Set<TEntity>();
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            return entity != null ? (IActionResult)Ok(entity) : NotFound();
        }

        public virtual async Task<IActionResult> Post(CancellationToken cancellationToken) {
            var entity = await ReadEntity(cancellationToken);

            this.OnCreating(entity);

            if (!this.ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var dbSet = this.Context.Set<TEntity>();
                await dbSet.AddAsync(entity, cancellationToken);
                await this.Context.SaveChangesAsync(cancellationToken);

                return Created(entity);
            } catch (DbUpdateException ex) {
                if (ArchiveDbContext.IsUserError(ex, out var message)) {
                    return BadRequest(new ProblemDetails {
                        Type = "archive-site:database-error/invalid-create",
                        Title = "The requested update was not valid.",
                        Detail = message
                    });
                } else {
                    throw;
                }
            }
        }

        public virtual async Task<IActionResult> Put([FromODataUri] Int64 key, CancellationToken cancellationToken) {
            var entity = await ReadEntity(cancellationToken);

            var dbSet = this.Context.Set<TEntity>();
            var existing = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            if (existing == null) {
                return NotFound();
            }

            this.OnUpdating(key, entity);

            entity.CopyTo(existing);
            try {
                await this.Context.SaveChangesAsync(cancellationToken);

                return Ok(existing);
            } catch (DbUpdateException ex) {
                if (ArchiveDbContext.IsUserError(ex, out var message)) {
                    return BadRequest(new ProblemDetails {
                        Type = "archive-site:database-error/invalid-update",
                        Title = "The requested update was not valid.",
                        Detail = message
                    });
                } else {
                    throw;
                }
            }
        }

        public virtual async Task<IActionResult> Patch([FromODataUri] Int64 key, Delta<TEntity> delta, CancellationToken cancellationToken) {
            var dbSet = this.Context.Set<TEntity>();
            var existing = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            if (existing == null) {
                return NotFound();
            }

            delta.Patch(existing);
            try {
                await this.Context.SaveChangesAsync(cancellationToken);

                return Ok(existing);
            } catch (DbUpdateException ex) {
                if (ArchiveDbContext.IsUserError(ex, out var message)) {
                    return BadRequest(new ProblemDetails {
                        Type = "archive-site:database-error/invalid-update",
                        Title = "The requested update was not valid.",
                        Detail = message
                    });
                } else {
                    throw;
                }
            }
        }

        public virtual async Task<IActionResult> Delete([FromODataUri] Int64 key, CancellationToken cancellationToken) {
            var dbSet = this.Context.Set<TEntity>();
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            if (entity != null) {
                dbSet.Remove(entity);
            }

            await this.Context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        protected async Task<TEntity> ReadEntity(CancellationToken cancellationToken) {
            // This is crap, but AspNet.OData does something to completely hose normal [FromBody] model binding.
            await using var body = this.Request.BodyReader.AsStream();
            var entity = await JsonSerializer.DeserializeAsync<TEntity>(body, this.SerializerOptions, cancellationToken: cancellationToken);
            this.TryValidateModel(entity);
            return entity;
        }

        protected virtual void OnCreating(TEntity entity) {
            if (entity.Id != 0) {
                this.ModelState.AddModelError(
                    nameof(entity),
                    "New entities should not have non-zero Ids specified. Ids are automatically generated."
                );
            }
        }

        protected virtual void OnUpdating(Int64 key, TEntity entity) {
            if (entity.Id != key) {
                this.ModelState.AddModelError(
                    nameof(entity),
                    $"The specified entity Id ({entity.Id}) does not match the Id being updated ({key}). The Id of an entity is read only."
                );
            }
        }
    }
}
