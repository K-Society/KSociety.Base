using Autofac;
using KSociety.Base.Infra.Shared.Class;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    public class DatabaseConfiguration : Module
    {
        private readonly DatabaseEngine _databaseEngine;
        private readonly string _masterString;
        private readonly bool _debugFlag;
        private readonly string _migrationsAssembly;

        public DatabaseConfiguration(DatabaseEngine databaseEngine, 
            string masterString, bool debugFlag)
        {
            _databaseEngine = databaseEngine;
            _masterString = masterString;
            _debugFlag = debugFlag;
            _migrationsAssembly = string.Empty;
        }

        public DatabaseConfiguration(DatabaseEngine databaseEngine,
            string masterString, bool debugFlag, string migrationsAssembly)
        {
            _databaseEngine = databaseEngine;
            _masterString = masterString;
            _debugFlag = debugFlag;
            _migrationsAssembly = migrationsAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var conf = new Infra.Shared.Class.DatabaseConfiguration(_databaseEngine, _masterString, _debugFlag, _migrationsAssembly);
            builder.RegisterInstance(conf).AsSelf().SingleInstance();
        }
    }
}
