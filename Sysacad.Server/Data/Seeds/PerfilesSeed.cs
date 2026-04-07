using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Seeds
{
    public class PerfilesSeed : IEntityTypeConfiguration<Perfil>
    {
        private static readonly DateTime SeedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Perfil> builder)
        {
            builder.HasData(
                new Perfil { Id = 1, Nombre = "Administrador", Descripcion = "Perfil con acceso total", CreatedById = 1, CreatedAt = SeedDate },
                new Perfil { Id = 2, Nombre = "Docente",       Descripcion = "Perfil para docentes",    CreatedById = 1, CreatedAt = SeedDate },
                new Perfil { Id = 3, Nombre = "Alumno",        Descripcion = "Perfil para alumnos",     CreatedById = 1, CreatedAt = SeedDate }
            );

            builder.HasMany(p => p.Usuarios)
                .WithMany(u => u.Perfiles)
                .UsingEntity<Dictionary<string, object>>(
                    "UsuarioPerfil",
                    j => j.HasOne<Usuario>().WithMany().HasForeignKey("UsuarioId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Perfil>().WithMany().HasForeignKey("PerfilId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("UsuarioId", "PerfilId");
                        j.ToTable("UsuarioPerfil");
                        j.HasData(new Dictionary<string, object> { ["UsuarioId"] = 1, ["PerfilId"] = 1 });
                    });

            builder.HasMany(p => p.Permisos)
                .WithMany(p => p.Perfiles)
                .UsingEntity<Dictionary<string, object>>(
                    "PerfilPermiso",
                    j => j.HasOne<Permiso>().WithMany().HasForeignKey("PermisoId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Perfil>().WithMany().HasForeignKey("PerfilId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("PerfilId", "PermisoId");
                        j.ToTable("PerfilPermiso");
                        j.HasData(
                            // Administrador → todos los permisos
                            new Dictionary<string, object> { ["PerfilId"] = 1, ["PermisoId"] = 1 },
                            new Dictionary<string, object> { ["PerfilId"] = 1, ["PermisoId"] = 2 },
                            new Dictionary<string, object> { ["PerfilId"] = 1, ["PermisoId"] = 3 },
                            new Dictionary<string, object> { ["PerfilId"] = 1, ["PermisoId"] = 4 },
                            // Docente
                            new Dictionary<string, object> { ["PerfilId"] = 2, ["PermisoId"] = 2 },
                            // Alumno
                            new Dictionary<string, object> { ["PerfilId"] = 3, ["PermisoId"] = 3 }
                        );
                    });
        }
    }
}

