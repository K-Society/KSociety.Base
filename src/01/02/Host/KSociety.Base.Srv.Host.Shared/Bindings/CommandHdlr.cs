// Copyright � K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Bindings
{
    using System.Linq;
    using Autofac;
    using KSociety.Base.App.Shared;
    using KSociety.Base.Srv.Shared.Class;
    using KSociety.Base.Srv.Shared.Interface;

    /// <summary>
    /// The Command Handler module for Autofac.
    /// </summary>
    public class CommandHdlr : Module
    {
        private readonly string[] _assemblies;

        public CommandHdlr(string[] assemblies)
        {
            this._assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CommandHandler>().As<ICommandHandler>();
            builder.RegisterType<CommandHandlerAsync>().As<ICommandHandlerAsync>();

            var assemblies = this._assemblies.Select(System.Reflection.Assembly.LoadFrom).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestListHandlerWithResponse<,,>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestListHandlerWithResponseAsync<,,>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestHandlerWithResponse<,>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestHandlerWithResponseAsync<,>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestHandler<>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestHandlerAsync<>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestHandlerWithResponse<>))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.Name.EndsWith("ReqHdlr"))
                .AsClosedTypesOf(typeof(IRequestHandlerWithResponseAsync<>))
                .AsImplementedInterfaces();
        }
    }
}
