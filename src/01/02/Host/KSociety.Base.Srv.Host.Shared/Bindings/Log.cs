// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    using Autofac;
    using Microsoft.Extensions.Logging;
    using Serilog.Extensions.Logging;

    /// <summary>
    /// 
    /// </summary>
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

            //builder.RegisterType<LoggerFactory>()
            //    .As<ILoggerFactory>()
            //    .SingleInstance();

            builder.Register(_ => new LoggerFactory(new ILoggerProvider[] {new SerilogLoggerProvider()}))
                .As<ILoggerFactory>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

        }
    }
}