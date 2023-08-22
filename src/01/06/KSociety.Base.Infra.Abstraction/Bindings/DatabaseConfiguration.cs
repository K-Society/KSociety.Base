// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Abstraction.Bindings
{
    using Autofac;
    using Class;
    using Interface;

    /// <summary>
    /// 
    /// </summary>
    public class DatabaseConfiguration : Module
    {
        private readonly DatabaseEngine _databaseEngine;
        private readonly string _masterString;
        private readonly bool _debugFlag;
        private readonly string _migrationsAssembly;
        private readonly bool _lazyLoading;

        public DatabaseConfiguration(DatabaseEngine databaseEngine,
            string masterString, bool debugFlag = false, string migrationsAssembly = "", bool lazyLoading = false)
        {
            this._databaseEngine = databaseEngine;
            this._masterString = masterString;
            this._debugFlag = debugFlag;
            this._migrationsAssembly = migrationsAssembly;
            this._lazyLoading = lazyLoading;
        }

        public DatabaseConfiguration(DatabaseOptions databaseOptions)
        {
            this._databaseEngine = databaseOptions.DatabaseEngine;
            this._masterString = databaseOptions.ConnectionString;
            this._debugFlag = databaseOptions.Logging;
            this._migrationsAssembly = databaseOptions.MigrationsAssembly;
            this._lazyLoading = databaseOptions.LazyLoading;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var conf = new Infra.Abstraction.Class.DatabaseConfiguration(this._databaseEngine, this._masterString, this._debugFlag, this._migrationsAssembly, this._lazyLoading);
            builder.RegisterInstance<IDatabaseConfiguration>(conf).SingleInstance();
        }
    }
}
