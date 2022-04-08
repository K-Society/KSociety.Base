using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent;
public class AgentCommandImportExport<
    TCommand, TCommandAsync,
    TImport, TImportAsync,
    TExport, TExportAsync,
    TAddReq, TAddRes,
    TUpdateReq, TUpdateRes,
    TCopyReq, TCopyRes,
    TModifyFieldReq, TModifyFieldRes,
    TRemoveReq, TRemoveRes,
    TImportReq, TImportRes,
    TExportReq, TExportRes> : AgentCommand<TCommand, TCommandAsync, TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>, 
    IAgentCommandImportExport<
    TAddReq, TAddRes,
    TUpdateReq, TUpdateRes,
    TCopyReq, TCopyRes,
    TModifyFieldReq, TModifyFieldRes,
    TRemoveReq, TRemoveRes,
    TImportReq, TImportRes,
    TExportReq, TExportRes>
    where TCommand : class, ICommand<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
    where TCommandAsync : class, ICommandAsync<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
    where TImport : class, IImport<TImportReq, TImportRes>
    where TImportAsync : class, IImportAsync<TImportReq, TImportRes>
    where TExport : class, IExport<TExportReq, TExportRes>
    where TExportAsync : class, IExportAsync<TExportReq, TExportRes>
    where TAddReq : class
    where TAddRes : class
    where TUpdateReq : class
    where TUpdateRes : class
    where TCopyReq : class
    where TCopyRes : class
    where TModifyFieldReq : class
    where TModifyFieldRes : class
    where TRemoveReq : class
    where TRemoveRes : class
    where TImportReq : class
    where TImportRes : class
    where TExportReq : class
    where TExportRes : class
{

    private readonly IAgentImport<TImportReq, TImportRes> _agentImport;
    private readonly IAgentExport<TExportReq, TExportRes> _agentExport;

    public AgentCommandImportExport(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        : base(agentConfiguration, loggerFactory)
    {
        _agentImport = new AgentImport<TImport, TImportAsync, TImportReq, TImportRes>(agentConfiguration, loggerFactory);
        _agentExport = new AgentExport<TExport, TExportAsync, TExportReq, TExportRes>(agentConfiguration, loggerFactory);
    }

    public virtual TImportRes ImportData(TImportReq request, CancellationToken cancellationToken = default)
    {
        return _agentImport.ImportData(request, cancellationToken);
    }

    public virtual async ValueTask<TImportRes> ImportDataAsync(TImportReq request, CancellationToken cancellationToken = default)
    {
        return await _agentImport.ImportDataAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public virtual TExportRes ExportData(TExportReq request, CancellationToken cancellationToken = default)
    {
        return _agentExport.ExportData(request, cancellationToken);
    }

    public virtual async ValueTask<TExportRes> ExportDataAsync(TExportReq request, CancellationToken cancellationToken = default)
    {
        return await _agentExport.ExportDataAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
