using Autofac;
using KSociety.Base.Infra.Shared.Class;
using KSociety.Base.Infra.Shared.Interface;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    public class UnitOfWork<TContext> : Module where TContext : KbDbContext
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<KbUnitOfWork<TContext>>().As<IDbUnitOfWork>();
        }
    }
}
