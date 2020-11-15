using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ArchiveSite.Data;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArchiveSiteBackend.Api.Controllers {
    [ApiController]
    [Route("api/odata/[controller]")]
    public abstract class EntityControllerBase<TContext, TEntity> : ODataController
        where TContext : DbContext
        where TEntity : EntityBase<TEntity> {
        protected TContext DbContext { get; }

        private readonly JsonSerializerOptions serializerOptions;
        private IDbContextTransaction transaction;

        protected virtual JsonSerializerOptions SerializerOptions => this.serializerOptions;

        protected EntityControllerBase(TContext context) {
            this.DbContext = context ?? throw new ArgumentNullException(nameof(context));

            this.serializerOptions = new JsonSerializerOptions();
            this.serializerOptions.Converters.Insert(0, new JsonStringEnumConverter());
        }

        [EnableQuery]
        [AllowAnonymous]
        public virtual IQueryable<TEntity> Get() {
            return this.DbContext.Set<TEntity>();
        }

        [EnableQuery]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Get(
            [FromODataUri] Int64 key,
            CancellationToken cancellationToken) {

            var dbSet = this.DbContext.Set<TEntity>();
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            return entity != null ? (IActionResult)Ok(entity) : NotFound();
        }

        public virtual async Task<IActionResult> Post(
            [FromBody] TEntity entity,
            CancellationToken cancellationToken) {

            this.transaction = await this.DbContext.Database.BeginTransactionAsync(cancellationToken);
            await this.OnCreating(entity, cancellationToken);

            if (!this.ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var dbSet = this.DbContext.Set<TEntity>();
            entity = (await dbSet.AddAsync(entity, cancellationToken)).Entity;

            return await this.TrySaveChanges(
                entity,
                async savedEntity => {
                    await this.OnCreated(savedEntity, cancellationToken);
                    return this.Created(savedEntity);
                },
                "create",
                cancellationToken
            );
        }

        public virtual async Task<IActionResult> Put(
            [FromODataUri] Int64 key,
            [FromBody] TEntity entity,
            CancellationToken cancellationToken) {

            this.transaction = await this.DbContext.Database.BeginTransactionAsync(cancellationToken);
            var dbSet = this.DbContext.Set<TEntity>();
            var existing = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            if (existing == null) {
                return NotFound();
            }

            await this.OnUpdating(key, existing, entity, cancellationToken);

            entity.CopyTo(existing);
            return await TrySaveChanges(
                existing,
                async updatedEntity => {
                    await this.OnUpdated(updatedEntity, cancellationToken);
                    return Ok(updatedEntity);
                },
                "update",
                cancellationToken
            );
        }

        public virtual async Task<IActionResult> Patch(
            [FromODataUri] Int64 key,
            Delta<TEntity> delta,
            CancellationToken cancellationToken) {

            this.transaction = await this.DbContext.Database.BeginTransactionAsync(cancellationToken);
            var dbSet = this.DbContext.Set<TEntity>();
            var existing = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            if (existing == null) {
                return NotFound();
            }

            delta.Patch(existing);
            return await TrySaveChanges(
                existing,
                async updatedEntity => {
                    await this.OnUpdated(updatedEntity, cancellationToken);
                    return Ok(updatedEntity);
                },
                "update",
                cancellationToken
            );
        }

        public virtual async Task<IActionResult> Delete(
            [FromODataUri] Int64 key,
            CancellationToken cancellationToken) {

            var dbSet = this.DbContext.Set<TEntity>();
            var entity = await dbSet.SingleOrDefaultAsync(e => e.Id == key, cancellationToken);

            if (entity != null) {
                dbSet.Remove(entity);
            }

            await this.DbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        protected virtual Task OnCreating(TEntity entity, CancellationToken cancellationToken) {
            if (entity.Id != 0) {
                this.ModelState.AddModelError(
                    nameof(entity),
                    "New entities should not have non-zero Ids specified. Ids are automatically generated."
                );
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Invoked after an entity has been created in the database.
        /// </summary>
        /// <param name="createdEntity">
        /// The entity that has been created.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token.
        /// </param>
        protected virtual Task OnCreated(
            TEntity createdEntity,
            CancellationToken cancellationToken) {

            return Task.CompletedTask;
        }

        protected virtual Task OnUpdating(
            Int64 key,
            TEntity existing,
            TEntity update,
            CancellationToken cancellationToken) {

            if (update.Id != key) {
                this.ModelState.AddModelError(
                    nameof(update),
                    $"The specified entity Id ({update.Id}) does not match the Id being updated ({key}). The Id of an entity is read only."
                );
            }

            return Task.CompletedTask;
        }

        protected virtual Task OnUpdated(
            TEntity updatedEntity,
            CancellationToken cancellationToken) {

            return Task.CompletedTask;
        }

        protected async Task<IActionResult> TrySaveChanges(
            TEntity entity,
            Func<TEntity, Task<IActionResult>> onSuccess,
            String operation,
            CancellationToken cancellationToken) {

            try {
                await this.DbContext.SaveChangesAsync(cancellationToken);

                var result = await onSuccess(entity);

                if (this.transaction != null) {
                    await this.transaction.CommitAsync(cancellationToken);
                }

                return result;
            } catch (DbUpdateException ex) {
                if (ArchiveDbContext.IsUserError(ex, out var message)) {
                    return BadRequest(new ProblemDetails {
                        Type = $"archive-site:database-error/invalid-{operation}",
                        Title = $"The requested {operation} was not valid.",
                        Detail = message
                    });
                } else {
                    throw;
                }
            }
        }
    }
}
