//using Autofac;
//using KSociety.Base.Infra.Abstraction.Interface;
//using KSociety.Base.Infra.Shared.Class;
//using KSociety.Base.Infra.Shared.Interface;
//using Microsoft.Extensions.Logging;

//namespace KSociety.Base.Srv.Host.Shared.Bindings
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <typeparam name="TContext"></typeparam>
//    public class DatabaseFactory<TContext> : Module where TContext : DatabaseContext
//    {
//        protected override void Load(ContainerBuilder builder)
//        {

//            //builder.RegisterType<DatabaseFactory<ILoggerFactory, Infra.Shared.Class.DatabaseConfiguration, TContext>>()
//            //    .As<IDatabaseFactory<TContext>>().InstancePerLifetimeScope();

//            builder.RegisterType<DatabaseFactory<ILoggerFactory, IDatabaseConfiguration, TContext>>()
//                .As<IDatabaseFactory<TContext>>().InstancePerLifetimeScope();
//        }
//    }
//}