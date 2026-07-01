

namespace SGA.Domain.Repository
{
    public interface IBaseRepository<TEntity, TModel>
    {
        Task<TModel?> GetByIdAsync(int id);
        Task<IReadOnlyList<TModel>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}