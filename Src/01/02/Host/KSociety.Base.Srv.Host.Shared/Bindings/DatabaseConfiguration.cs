using Autofac;
using KSociety.Base.Infra.Shared.Class;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    public class DatabaseConfiguration : Module
    {
        private readonly string _masterString;
        private readonly bool _debugFlag;
        private readonly string _migrationsAssembly;

        public DatabaseConfiguration(string masterString, bool debugFlag, string migrationsAssembly)
        {
            _masterString = masterString;
            _debugFlag = debugFlag;
            _migrationsAssembly = migrationsAssembly;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var conf = new DatabaseConfiguration(_masterString, _debugFlag, _migrationsAssembly);
            builder.RegisterInstance(conf).AsSelf().SingleInstance();
        }
    }
}
