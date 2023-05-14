using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public class AgentExportBase<TExport, TExportReq, TExportRes> : Connection,
        IAgentExportBase<TExportReq, TExportRes>
        where TExport : class, IExport<TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        public AgentExportBase(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {

        }

        public virtual TExportRes ExportData(TExportReq request,
            CancellationToken cancellationToken = default)
        {
            TExportRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TExport>();

                    var result = client.ExportData(request, ConnectionOptions(cancellationToken));

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
