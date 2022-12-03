using Autofac;
using KSociety.Base.Infra.Abstraction.Interface;
using KSociety.Base.Infra.Shared.Class;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Bindings
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class DatabaseControl<TContext> : Module where TContext : DatabaseContext
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DatabaseFactory<ILoggerFactory, IDatabaseConfiguration, TContext>>()
                .As<IDatabaseFactory<TContext>>().InstancePerLifetimeScope();
        }
    }
}