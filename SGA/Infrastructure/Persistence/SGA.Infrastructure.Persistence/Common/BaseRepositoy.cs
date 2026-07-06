using Microsoft.EntityFrameworkCore;
using SGA.Domain.Base;
using SGA.Domain.Repository;
using SGA.Infrastructure.Persistence.Data;
using System.Linq.Expressions;

namespace SGA.Infrastructure.Persistence.Common
{
    public abstract class BaseRepository<TEntity, TModel> : IBaseRepository<TEntity, TModel>
        where TEntity : BaseEntity
        where TModel : class
    {
        protected readonly SgaDbContext Context;
        protected DbSet<TEntity> Set => Context.Set<TEntity>();

        protected BaseRepository(SgaDbContext context) => Context = context;

        protected abstract Expression<Func<TEntity, TModel>> Proyeccion { get; }

        public virtual async Task<TModel?> GetByIdAsync(int id) =>
            await Set.AsNoTracking()
            .Where(e => e.Id == id)
            .Select(Proyeccion)
            .FirstOrDefaultAsync();

        public virtual async Task<IReadOnlyList<TModel>> GetAllAsync() =>
            await Set.AsNoTracking()
            .Select(Proyeccion)
            .ToListAsync();

        public virtual async Task AddAsync(TEntity entity)
        {
            await Set.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            Set.Update(entity);
            await Context.SaveChangesAsync();
        }
        public virtual async Task DeleteAsync(TEntity entity)
        {
            entity.Eliminado = true;
            entity.FechaEliminacion = DateTime.Now;
            entity.FechaEliminacion = DateTime.Now;
            Context.Attach(entity);
            Context.Entry(entity).Property(e => e.Eliminado).IsModified = true;
            Context.Entry(entity).Property(e => e.FechaEliminacion).IsModified = true;
            Context.Entry(entity).Property(e => e.EliminadoPor).IsModified = true;
            await Context.SaveChangesAsync();
        }
    }
}