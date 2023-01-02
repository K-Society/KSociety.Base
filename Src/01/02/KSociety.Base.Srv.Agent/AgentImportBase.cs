using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public class AgentImportBase<TImport, TImportReq, TImportRes> : Connection,
        IAgentImportBase<TImportReq, TImportRes>
        where TImport : class, IImport<TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        public AgentImportBase(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {

        }

        public virtual TImportRes ImportData(TImportReq request,
            CancellationToken cancellationToken = default)
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
                Logger.LogError(ex, "{0}.{1}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return output;
        }
    }
}
