using Microsoft.EntityFrameworkCore;

using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>
    {
        public UsuarioRepository(ApiDbContext context, ILogger<GenericRepository<Usuario>> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Obtiene un usuario por su username
        /// </summary>
        /// <param name="username">Username del usuario</param>
        /// <param name="borradoLogico">Si es true, incluye usuarios eliminados lógicamente</param>
        public async Task<Usuario?> GetByUsernameAsync(string username, bool borradoLogico = false)
        {
            var query = _dbSet.Where(u => u.Username == username);

            if (!borradoLogico)
            {
                query = query.Where(u => u.BorradoLogico == false);
            }

            return await query.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Verifica si existe un usuario con el username especificado
        /// </summary>
        /// <param name="username">Username a verificar</param>
        /// <param name="borradoLogico">Si es true, incluye usuarios eliminados lógicamente</param>
        public async Task<bool> ExistsAsync(string username, bool borradoLogico = false)
        {
            var query = _dbSet.Where(u => u.Username == username);

            if (!borradoLogico)
            {
                query = query.Where(u => u.BorradoLogico == false);
            }

            return await query.AnyAsync();
        }
    }
}

