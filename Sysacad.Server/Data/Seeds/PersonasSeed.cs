using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data.Entities;

namespace Sysacad.Server.Data.Seeds
{
    public class PersonasSeed : IEntityTypeConfiguration<Persona>
    {
        private static readonly DateTime SeedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Persona> builder)
        {
            builder.HasData(
                new Persona
                {
                    Id = 1,
                    Nombres = "Agustin",
                    Apellidos = "Gigena",
                    Documento = "445590912",
                    FechaNacimiento = new DateOnly(2003, 4, 17),
                    CreatedById = 1,
                    CreatedAt = SeedDate
                }
            );
        }
    }
}
