using KSociety.Base.Infra.Shared.Interface;
using System.Text;

namespace KSociety.Base.Infra.Shared.Class
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public DatabaseEngine DatabaseEngine { get; }

        public string ConnectionString { get; }

        public bool Logging { get;  }

        public string MigrationsAssembly { get; }

        public DatabaseConfiguration(DatabaseEngine databaseEngine, 
            string connectionString, bool logging, string migrationsAssembly = "")
        {
            DatabaseEngine = databaseEngine;
            ConnectionString = connectionString;
            Logging = logging;
            MigrationsAssembly = migrationsAssembly;
        }
    }
}
