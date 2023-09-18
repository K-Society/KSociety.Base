// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentExportAsync<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        ValueTask<TExportRes?> ExportDataAsync(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}
