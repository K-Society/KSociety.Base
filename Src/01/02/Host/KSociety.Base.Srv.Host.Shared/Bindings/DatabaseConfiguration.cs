using Autofac;
using KSociety.Base.Infra.Shared.Class;
using KSociety.Base.Infra.Shared.Interface;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
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
            _databaseEngine = databaseEngine;
            _masterString = masterString;
            _debugFlag = debugFlag;
            _migrationsAssembly = migrationsAssembly;
            _lazyLoading = lazyLoading;
        }

        public DatabaseConfiguration(DatabaseOptions databaseOptions)
        {
            _databaseEngine = databaseOptions.DatabaseEngine;
            _masterString = databaseOptions.ConnectionString;
            _debugFlag = databaseOptions.Logging;
            _migrationsAssembly = databaseOptions.MigrationsAssembly;
            _lazyLoading = databaseOptions.LazyLoading;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var conf = new Infra.Shared.Class.DatabaseConfiguration(_databaseEngine, _masterString, _debugFlag,
                _migrationsAssembly, _lazyLoading);
            builder.RegisterInstance<IDatabaseConfiguration>(conf).SingleInstance();
        }
    }
}