using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Seeds
{
    public class UsuariosSeed : IEntityTypeConfiguration<Usuario>
    {
        private static readonly DateTime SeedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Usuario> builder)
        {
            builder.HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "agigena",
#pragma warning disable S2068 // Hard-coded seed password hash for development
                    PasswordHash = "$2a$11$10NVA7ARBLFPHK9MhJxMa.viPSrdT118usXndVngIlVeeuFADA4vK",
#pragma warning restore S2068
                    Email = "agustingigena1704@gmail.com",
                    LastLogin = SeedDate,
                    PersonaId = 1,
                    CreatedById = 1,
                    CreatedAt = SeedDate
                }
            );
        }
    }
}
