using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Contract;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Behavior
{
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
        KSociety.Base.Srv.Contract.ICommandImportExport<
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
            _import = new Import<TImportReq, TImportRes>(loggerFactory, componentContext, commandHandler);
            _export = new Export<TExportReq, TExportRes>(loggerFactory, componentContext, commandHandler);
        }

        public virtual TImportRes ImportData(TImportReq importReq, CallContext context = default)
        {
            return _import.ImportData(importReq, context);
        }

        public virtual TExportRes ExportData(TExportReq exportReq, CallContext context = default)
        {
            return _export.ExportData(exportReq, context);
        }
    }
}
