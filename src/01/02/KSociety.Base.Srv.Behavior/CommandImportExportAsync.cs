// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior
{
    using Autofac;
    using KSociety.Base.App.Shared;
    using Contract;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;
    using System.Threading.Tasks;

    public class CommandImportExportAsync<
        TAddReq, TAddRes,
        TUpdateReq, TUpdateRes,
        TCopyReq, TCopyRes,
        TModifyFieldReq, TModifyFieldRes,
        TRemoveReq, TRemoveRes,
        TImportReq, TImportRes,
        TExportReq, TExportRes> : CommandAsync<
            TAddReq, TAddRes,
            TUpdateReq, TUpdateRes,
            TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes,
            TRemoveReq, TRemoveRes>,
        ICommandImportExportAsync<
            TAddReq, TAddRes,
            TUpdateReq, TUpdateRes,
            TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes,
            TRemoveReq, TRemoveRes,
            TImportReq, TImportRes,
            TExportReq, TExportRes>
        where TAddReq : class, IRequest, new()
        where TAddRes : class, IResponse, new()
        where TUpdateReq : class, IRequest, new()
        where TUpdateRes : class, IResponse, new()
        where TCopyReq : class, IRequest, new()
        where TCopyRes : class, IResponse, new()
        where TModifyFieldReq : class, IRequest, new()
        where TModifyFieldRes : class, IResponse, new()
        where TRemoveReq : class, IRequest, new()
        where TRemoveRes : class, IResponse, new()
        where TImportReq : class, IRequest, new()
        where TImportRes : class, IResponse, new()
        where TExportReq : class, IRequest, new()
        where TExportRes : class, IResponse, new()
    {
        private readonly IImportAsync<TImportReq, TImportRes> _importAsync;
        private readonly IExportAsync<TExportReq, TExportRes> _exportAsync;

        public CommandImportExportAsync(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext,
            ICommandHandlerAsync commandHandlerAsync
        ) : base(loggerFactory, componentContext, commandHandlerAsync)
        {
            this._importAsync =
                new ImportAsync<TImportReq, TImportRes>(loggerFactory, componentContext, commandHandlerAsync);
            this._exportAsync =
                new ExportAsync<TExportReq, TExportRes>(loggerFactory, componentContext, commandHandlerAsync);
        }

        public virtual async ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CallContext context = default)
        {
            return await this._importAsync.ImportDataAsync(importReq, context).ConfigureAwait(false);
        }

        public virtual async ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CallContext context = default)
        {
            return await this._exportAsync.ExportDataAsync(exportReq, context).ConfigureAwait(false);
        }
    }
}
