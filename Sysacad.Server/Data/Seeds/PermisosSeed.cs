using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Seeds
{
    public class PermisosSeed : IEntityTypeConfiguration<Permiso>
    {
        private static readonly DateTime SeedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Permiso> builder)
        {
            builder.HasData(
                new Permiso { Id = 1, Nombre = "Administrador", Codigo = "admin", Descripcion = "Acceso total al sistema", CreatedById = 1, CreatedAt = SeedDate },
                new Permiso { Id = 2, Nombre = "Docente",       Codigo = "docente",      Descripcion = "Acceso a funciones docentes", CreatedById = 1, CreatedAt = SeedDate },
                new Permiso { Id = 3, Nombre = "Alumno",        Codigo = "alumno",       Descripcion = "Acceso a funciones de alumno", CreatedById = 1, CreatedAt = SeedDate },
                new Permiso { Id = 4, Nombre = "Usuarios",      Codigo = "usuarios",     Descripcion = "Gestión de usuarios", CreatedById = 1, CreatedAt = SeedDate }
            );
        }
    }
}
