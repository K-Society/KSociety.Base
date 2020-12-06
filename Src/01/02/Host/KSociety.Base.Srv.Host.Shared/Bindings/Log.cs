using Autofac;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    public class Log : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<LoggerFactory>().As<ILoggerFactory>().SingleInstance();
            //// Create Logger<T> when ILogger<T> is required.
            //builder.RegisterGeneric(typeof(Logger<>))
            //    .As(typeof(ILogger<>));

            //// Use NLogLoggerFactory as a factory required by Logger<T>.
            //builder.RegisterType<NLogLoggerFactory>()
            //    .AsImplementedInterfaces().InstancePerLifetimeScope();


            builder.Register(_ => new LoggerFactory(new ILoggerProvider[] { new SerilogLoggerProvider() }))
                .As<ILoggerFactory>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

        }
    }
}
