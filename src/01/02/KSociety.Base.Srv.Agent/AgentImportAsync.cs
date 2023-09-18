// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using Contract;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AgentImportAsync<TImportAsync, TImportReq, TImportRes> : Connection,
        IAgentImportAsync<TImportReq, TImportRes>
        where TImportAsync : class, IImportAsync<TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        public AgentImportAsync(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {

        }

        public virtual async ValueTask<TImportRes?> ImportDataAsync(TImportReq request,
            CancellationToken cancellationToken = default)
        {
            TImportRes? output = default;
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TImportAsync>();

                    if (client != null)
                    {
                        var result = await client.ImportDataAsync(request, this.ConnectionOptions(cancellationToken))
                            .ConfigureAwait(false);

                        output = result;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return output;
        }
    }
}
