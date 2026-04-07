using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Repositories
{
    public class PersonaRepository : GenericRepository<Persona>
    {
        public PersonaRepository(ApiDbContext context, ILogger<PersonaRepository> logger)
            : base(context, logger)
        {
        }
    }
}
