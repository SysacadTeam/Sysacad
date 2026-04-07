using Microsoft.EntityFrameworkCore;
using Sysacad.Server.Data;

namespace Sysacad.Server.Services
{
    public static class DbConfigurationService
    {
        public static void ConfigureDevelopment(WebApplicationBuilder builder)
        {
            var dbSection = builder.Configuration.GetSection("Database");

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST")
                        ?? dbSection["Host"]
                        ?? throw new InvalidOperationException("Database Host no configurado.");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT")
                        ?? dbSection["Port"]
                        ?? "5432";
            var dbName = Environment.GetEnvironmentVariable("DB_NAME")
                        ?? dbSection["Name"]
                        ?? throw new InvalidOperationException("Database Name no configurado.");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER")
                        ?? dbSection["User"]
                        ?? throw new InvalidOperationException("Database User no configurado.");
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS")
                        ?? dbSection["Pass"]
                        ?? throw new InvalidOperationException("Database Password no configurado.");

            var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User Id={dbUser};Password={dbPass};SSL Mode=Disable;";

            builder.Services.AddDbContext<ApiDbContext>(opt =>
                opt.UseLazyLoadingProxies().UseNpgsql(connectionString));
        }

        public static void ConfigureProduction(WebApplicationBuilder builder)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST")
                        ?? throw new InvalidOperationException("DB_HOST no configurado.");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT")
                        ?? throw new InvalidOperationException("DB_PORT no configurado.");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME")
                        ?? throw new InvalidOperationException("DB_NAME no configurado.");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER")
                        ?? throw new InvalidOperationException("DB_USER no configurado.");
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS")
                        ?? throw new InvalidOperationException("DB_PASS no configurado.");

            var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User Id={dbUser};Password={dbPass};";

            var certPath = Environment.GetEnvironmentVariable("DB_CERTIFICATE_PATH");
            if (!string.IsNullOrEmpty(certPath))
            {
                connectionString += $"SSL Mode=Require;Root Certificate={certPath};";
            }

            builder.Services.AddDbContext<ApiDbContext>(opt =>
                opt.UseLazyLoadingProxies().UseNpgsql(connectionString));
        }
    }
}
