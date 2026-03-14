using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Buffers.Text;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using static System.Net.WebRequestMethods;

namespace Sysacad.Server.Configuration
{
    public class ENV_SETUP
    {
        public static void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable("DB_HOST", "localhost");
            Environment.SetEnvironmentVariable("DB_NAME", "mydb");
            Environment.SetEnvironmentVariable("DB_PASS", "postgres");
            Environment.SetEnvironmentVariable("DB_PORT", "5432");
            Environment.SetEnvironmentVariable("DB_USER", "postgres");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "Sysacad.Server.Clients");
            Environment.SetEnvironmentVariable("JWT_EXPIRATION_MINUTES", "60");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "Sysacad.Server");
            Environment.SetEnvironmentVariable("JWT_KEEP_ALIVE_MINUTES", "5");
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "YourSuperSecretKeyThatIsAtLeast32CharsLong!");
        }
    }
}


