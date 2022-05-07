﻿using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent;
public class AgentImport<TImport, TImportAsync, TImportReq, TImportRes> : Connection, IAgentImport<TImportReq, TImportRes>
    where TImport : class, IImport<TImportReq, TImportRes>
    where TImportAsync : class, IImportAsync<TImportReq, TImportRes>
    where TImportReq : class
    where TImportRes : class
{
    public AgentImport(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
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
                var client = Channel.CreateGrpcService<TImport>();

                var result = client.ImportData(request, ConnectionOptions(cancellationToken));

                output = result;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
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
                var client = Channel.CreateGrpcService<TImportAsync>();

                var result = await client.ImportDataAsync(request, ConnectionOptions(cancellationToken));

                output = result;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        }
        return output;
    }
}