// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Behavior
{
    using Autofac;
    using KSociety.Base.App.Shared;
    using Contract;
    using Shared.Interface;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;

    public class CommandImportExport<
        TAddReq, TAddRes,
        TUpdateReq, TUpdateRes,
        TCopyReq, TCopyRes,
        TModifyFieldReq, TModifyFieldRes,
        TRemoveReq, TRemoveRes,
        TImportReq, TImportRes,
        TExportReq, TExportRes> : Command<
            TAddReq, TAddRes,
            TUpdateReq, TUpdateRes,
            TCopyReq, TCopyRes,
            TModifyFieldReq, TModifyFieldRes,
            TRemoveReq, TRemoveRes>,
        ICommandImportExport<
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

        private readonly IImport<TImportReq, TImportRes> _import;
        private readonly IExport<TExportReq, TExportRes> _export;

        public CommandImportExport(
            ILoggerFactory loggerFactory,
            IComponentContext componentContext,
            ICommandHandler commandHandler
        ) : base(loggerFactory, componentContext, commandHandler)
        {
            this._import = new Import<TImportReq, TImportRes>(loggerFactory, componentContext, commandHandler);
            this._export = new Export<TExportReq, TExportRes>(loggerFactory, componentContext, commandHandler);
        }

        public virtual TImportRes? ImportData(TImportReq importReq, CallContext context = default)
        {
            return this._import.ImportData(importReq, context);
        }

        public virtual TExportRes? ExportData(TExportReq exportReq, CallContext context = default)
        {
            return this._export.ExportData(exportReq, context);
        }
    }
}
