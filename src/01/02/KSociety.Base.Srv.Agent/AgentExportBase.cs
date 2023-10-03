// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using Contract;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;

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

        public virtual TExportRes? ExportData(TExportReq request,
            CancellationToken cancellationToken = default)
        {
            TExportRes? output = default;
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TExport>();

                    if (client != null)
                    {
                        var result = client.ExportData(request, this.ConnectionOptions(cancellationToken));

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
