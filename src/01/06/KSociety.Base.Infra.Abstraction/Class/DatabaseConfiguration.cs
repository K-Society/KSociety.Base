// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Abstraction.Class
{
    using Interface;

    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        public DatabaseEngine DatabaseEngine { get; }
        public string? ConnectionString { get; }
        public bool Logging { get; }
        public string? MigrationsAssembly { get; }
        public bool LazyLoading { get; }

        public DatabaseConfiguration(DatabaseEngine databaseEngine,
            string? connectionString, bool logging = false,
            string? migrationsAssembly = "", bool lazyLoading = false)
        {
            this.DatabaseEngine = databaseEngine;
            this.ConnectionString = connectionString;
            this.Logging = logging;
            this.MigrationsAssembly = migrationsAssembly;
            this.LazyLoading = lazyLoading;
        }

        public DatabaseConfiguration(DatabaseOptions databaseOptions)
        {
            this.DatabaseEngine = databaseOptions.DatabaseEngine;
            this.ConnectionString = databaseOptions.ConnectionString;
            this.Logging = databaseOptions.Logging;
            this.MigrationsAssembly = databaseOptions.MigrationsAssembly;
            this.LazyLoading = databaseOptions.LazyLoading;
        }

        public override string ToString()
        {
            return this.DatabaseEngine + " " + this.ConnectionString + " " + this.Logging + " " + this.MigrationsAssembly + " " + this.LazyLoading;
        }
    }
}
