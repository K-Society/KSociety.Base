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
        private readonly string _version;

        public DatabaseConfiguration(DatabaseEngine databaseEngine, 
            string masterString, bool debugFlag, string migrationsAssembly = "", string version = "")
        {
            _databaseEngine = databaseEngine;
            _masterString = masterString;
            _debugFlag = debugFlag;
            _migrationsAssembly = migrationsAssembly;
            _version = version;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var conf = new Infra.Shared.Class.DatabaseConfiguration(_databaseEngine, _masterString, _debugFlag, _migrationsAssembly, _version);
            builder.RegisterInstance(conf).AsSelf().SingleInstance();
        }
    }
}
