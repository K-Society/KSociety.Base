using KSociety.Base.Infra.Shared.Interface;

namespace KSociety.Base.Infra.Shared.Class
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public string ConnectionString { get; }

        public bool Logging { get;  }

        public string MigrationsAssembly { get; }

        public DatabaseConfiguration(string connectionString, bool logging, string migrationsAssembly = "")
        {
            ConnectionString = connectionString;
            Logging = logging;
            MigrationsAssembly = migrationsAssembly;
        }

    }
}
