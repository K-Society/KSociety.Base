using KSociety.Base.Infra.Shared.Interface;

namespace KSociety.Base.Infra.Shared.Class
{
    public class KbDatabaseConfiguration : IDatabaseConfiguration
    {
        public string ConnectionString { get; }

        public bool Logging { get;  }

        public string MigrationsAssembly { get; }

        public KbDatabaseConfiguration(string connectionString, bool logging, string migrationsAssembly = "")
        {
            ConnectionString = connectionString;
            Logging = logging;
            MigrationsAssembly = migrationsAssembly;
        }

    }
}
