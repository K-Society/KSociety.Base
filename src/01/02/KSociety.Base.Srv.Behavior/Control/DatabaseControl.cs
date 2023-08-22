// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior.Control
{
    using Autofac;
    using KSociety.Base.App.Utility.Dto.Res.Control;
    using KSociety.Base.Srv.Contract.Control;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;

    public class DatabaseControl : IDatabaseControl
    {
        private readonly ILogger<DatabaseControl> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IComponentContext _componentContext;
        private readonly ICommandHandler _commandHandler;

        public DatabaseControl(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext,
            ICommandHandler commandHandler
        )
        {
            this._loggerFactory = loggerFactory;
            this._componentContext = componentContext;
            this._commandHandler = commandHandler;
            this._logger = this._loggerFactory.CreateLogger<DatabaseControl>();
        }

        public EnsureCreated EnsureCreated(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return this._commandHandler.ExecuteWithResponse<EnsureCreated>(this._loggerFactory, this._componentContext);
        }

        public EnsureDeleted EnsureDeleted(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return this._commandHandler.ExecuteWithResponse<EnsureDeleted>(this._loggerFactory, this._componentContext);
        }

        public void Migration(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            this._commandHandler.Execute(this._loggerFactory, this._componentContext, "MigrationReqHdlr");
        }

        public ConnectionString GetConnectionString(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return this._commandHandler.ExecuteWithResponse<ConnectionString>(this._loggerFactory, this._componentContext);
        }
    }
}
