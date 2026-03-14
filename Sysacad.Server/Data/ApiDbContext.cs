using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data.Entities;
using System.Reflection;

namespace Sysacad.Server.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureAuditoryEntities(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);

                entity.HasMany(p => p.Usuarios)
                    .WithMany(u => u.Perfiles)
                    .UsingEntity<Dictionary<string, object>>(
                        "UsuarioPerfil",
                        j => j.HasOne<Usuario>().WithMany().HasForeignKey("UsuarioId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Perfil>().WithMany().HasForeignKey("PerfilId").OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("UsuarioId", "PerfilId");
                            j.ToTable("UsuarioPerfil");
                        });
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Codigo).IsUnique();

                entity.HasMany(p => p.Perfiles)
                    .WithMany(u => u.Permisos)
                    .UsingEntity<Dictionary<string, object>>(
                        "UsuarioPermiso",
                        j => j.HasOne<Perfil>().WithMany().HasForeignKey("PerfilId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Permiso>().WithMany().HasForeignKey("PermisoId").OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.HasKey("PerfilId", "PermisoId");
                            j.ToTable("PerfilPermiso");
                        });
            });
        }

        /// <summary>
        /// Configura automáticamente las relaciones de auditoría para todas las entidades que implementan IAudithory
        /// </summary>
        private void ConfigureAuditoryEntities(ModelBuilder modelBuilder)
        {
            var auditoryEntityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass
                         && !t.IsAbstract
                         && typeof(IAudithory).IsAssignableFrom(t)
                         && typeof(GenericEntity).IsAssignableFrom(t))
                .ToList();

            foreach (var entityType in auditoryEntityTypes)
            {
                var entity = modelBuilder.Model.FindEntityType(entityType);

                if (entity == null)
                {
                    modelBuilder.Entity(entityType);
                    entity = modelBuilder.Model.FindEntityType(entityType);
                }

                if (entity != null)
                {
                    modelBuilder.Entity(entityType)
                        .HasOne(typeof(Usuario), "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                    modelBuilder.Entity(entityType)
                        .HasOne(typeof(Usuario), "UpdatedBy")
                        .WithMany()
                        .HasForeignKey("UpdatedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired(false);
                    modelBuilder.Entity(entityType)
                        .HasOne(typeof(Usuario), "DeletedBy")
                        .WithMany()
                        .HasForeignKey("DeletedById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired(false);

                    modelBuilder.Entity(entityType)
                        .Property("BorradoLogico")
                        .HasDefaultValue(false);

                    modelBuilder.Entity(entityType)
                        .Property("CreatedAt")
                        .IsRequired();

                    modelBuilder.Entity(entityType)
                        .Property("UpdatedAt")
                        .IsRequired(false);

                    modelBuilder.Entity(entityType)
                        .Property("DeletedAt")
                        .IsRequired(false);
                }
            }
        }
    }
}
