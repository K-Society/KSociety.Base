namespace KSociety.Base.Infra.Shared.Bindings
{
    using Autofac;
    using KSociety.Base.Infra.Abstraction.Interface;
    using Class;
    using Interface;
    using Microsoft.Extensions.Logging;

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
