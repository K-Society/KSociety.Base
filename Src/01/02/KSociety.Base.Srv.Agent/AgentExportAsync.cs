using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public class AgentExportAsync<TExportAsync, TExportReq, TExportRes> : Connection,
        IAgentExportAsync<TExportReq, TExportRes>
        where TExportAsync : class, IExportAsync<TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        public AgentExportAsync(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {

        }

        public virtual async ValueTask<TExportRes> ExportDataAsync(TExportReq request,
            CancellationToken cancellationToken = default)
        {
            TExportRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TExportAsync>();

                    var result = await client.ExportDataAsync(request, ConnectionOptions(cancellationToken));

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return output;
        }
    }
}
