using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent;
public class AgentCommandImportExport<
    TCommand, TCommandAsync,
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
    where TCommand : class, ICommandImportExport<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes, TImportReq, TImportRes, TExportReq, TExportRes>
    where TCommandAsync : class, ICommandImportExportAsync<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes, TImportReq, TImportRes, TExportReq, TExportRes>
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
    public AgentCommandImportExport(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        : base(agentConfiguration, loggerFactory)
    {

    }

    public virtual TImportRes ImportData(TImportReq request, CancellationToken cancellationToken = default)
    {
        TImportRes output = default;
        try
        {
            using (Channel)
            {
                var client = Channel.CreateGrpcService<TCommand>();

                var result = client.ImportData(request, ConnectionOptions(cancellationToken));

                output = result;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }
        return output;
    }

    public virtual async ValueTask<TImportRes> ImportDataAsync(TImportReq request, CancellationToken cancellationToken = default)
    {
        TImportRes output = default;
        try
        {
            using (Channel)
            {
                var client = Channel.CreateGrpcService<TCommandAsync>();

                var result = await client.ImportDataAsync(request, ConnectionOptions(cancellationToken));

                output = result;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }
        return output;
    }

    public virtual TExportRes ExportData(TExportReq request, CancellationToken cancellationToken = default)
    {
        TExportRes output = default;
        try
        {
            using (Channel)
            {
                var client = Channel.CreateGrpcService<TCommand>();

                var result = client.ExportData(request, ConnectionOptions(cancellationToken));

                output = result;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }
        return output;
    }

    public virtual async ValueTask<TExportRes> ExportDataAsync(TExportReq request, CancellationToken cancellationToken = default)
    {
        TExportRes output = default;
        try
        {
            using (Channel)
            {
                var client = Channel.CreateGrpcService<TCommandAsync>();

                var result = await client.ExportDataAsync(request, ConnectionOptions(cancellationToken));

                output = result;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
        }
        return output;
    }
}
