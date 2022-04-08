using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Contract;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Behavior;
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
    KSociety.Base.Srv.Contract.ICommandImportExportAsync<
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
        _importAsync = new ImportAsync<TImportReq, TImportRes>(loggerFactory, componentContext, commandHandlerAsync);
        _exportAsync = new ExportAsync<TExportReq, TExportRes>(loggerFactory, componentContext, commandHandlerAsync);
    }

    public virtual async ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CallContext context = default)
    {
        return await _importAsync.ImportDataAsync(importReq, context).ConfigureAwait(false);
    }

    public virtual async ValueTask<TExportRes> ExportDataAsync(TExportReq exportReq, CallContext context = default)
    {
        return await _exportAsync.ExportDataAsync(exportReq, context).ConfigureAwait(false);
    }
}
