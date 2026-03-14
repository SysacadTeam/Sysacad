using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : GenericEntity
    {
        Task<TEntity?> GetByIdAsync(int id, bool borradoLogico = false);
        Task<IEnumerable<TEntity>> GetAllAsync(bool borradoLogico = false);
        Task<TEntity> AddAsync(TEntity entity, Usuario? by = null);
        Task<TEntity> UpdateAsync(TEntity entity, Usuario? by = null);
        Task<bool> DeleteAsync(int id, Usuario? by = null);
    }
}
