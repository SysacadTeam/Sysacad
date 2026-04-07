using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data.Entities;
using Sysacad.Server.Data.Seeds;
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
        public DbSet<Persona> Personas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureAuditoryEntities(modelBuilder);

            Seed(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Username).IsUnique();

                entity.HasOne(u => u.Persona)
                    .WithOne()
                    .HasForeignKey<Usuario>(u => u.PersonaId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });

            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
                entity.HasIndex(e => e.Codigo).IsUnique();
            });

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Documento).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.Documento).IsUnique();
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Domicilio).HasMaxLength(200);
                entity.Property(e => e.BorradoLogico).HasDefaultValue(false);
            });
        }

        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PermisosSeed());
            modelBuilder.ApplyConfiguration(new PerfilesSeed());
            modelBuilder.ApplyConfiguration(new PersonasSeed());
            modelBuilder.ApplyConfiguration(new UsuariosSeed());
        }

        /// <summary>
        /// Configura automáticamente las relaciones de auditoría para todas las entidades que implementan IAudithory
        /// </summary>
        private static void ConfigureAuditoryEntities(ModelBuilder modelBuilder)
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
