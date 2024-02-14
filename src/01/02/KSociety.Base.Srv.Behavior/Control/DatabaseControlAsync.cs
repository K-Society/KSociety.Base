// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior.Control
{
    using System.Threading.Tasks;
    using Autofac;
    using KSociety.Base.App.Utility.Dto.Res.Control;
    using KSociety.Base.Srv.Contract.Control;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;

    public class DatabaseControlAsync : IDatabaseControlAsync
    {
        private readonly ILogger<DatabaseControlAsync> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IComponentContext _componentContext;
        private readonly ICommandHandlerAsync _commandHandlerAsync;

        public DatabaseControlAsync(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext,
            ICommandHandlerAsync commandHandlerAsync
        )
        {
            this._loggerFactory = loggerFactory;
            this._componentContext = componentContext;
            this._commandHandlerAsync = commandHandlerAsync;
            this._logger = this._loggerFactory.CreateLogger<DatabaseControlAsync>();
        }

        public async ValueTask<EnsureCreated> EnsureCreatedAsync(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return await this._commandHandlerAsync
                .ExecuteWithResponseAsync<EnsureCreated>(this._loggerFactory, this._componentContext, context.CancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask<EnsureDeleted> EnsureDeletedAsync(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return await this._commandHandlerAsync
                .ExecuteWithResponseAsync<EnsureDeleted>(this._loggerFactory, this._componentContext, context.CancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask MigrationAsync(CallContext context = default)
        {
            this._logger.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            await this._commandHandlerAsync
                .ExecuteAsync(this._loggerFactory, this._componentContext, "MigrationReqHdlr", context.CancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask<ConnectionString> GetConnectionStringAsync(CallContext context = default)
        {
            this._logger.LogTrace(this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            return await this._commandHandlerAsync
                .ExecuteWithResponseAsync<ConnectionString>(this._loggerFactory, this._componentContext).ConfigureAwait(false);
        }
    }
}
