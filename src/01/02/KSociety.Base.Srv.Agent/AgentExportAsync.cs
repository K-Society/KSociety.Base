namespace KSociety.Base.Srv.Agent
{
    using Contract;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<TExportAsync>();

                    var result = await client.ExportDataAsync(request, this.ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    output = result;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return output;
        }
    }
}
