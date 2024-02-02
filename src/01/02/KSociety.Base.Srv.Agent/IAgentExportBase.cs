// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using System.Threading;

    public interface IAgentExportBase<in TExportReq, out TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        TExportRes ExportData(TExportReq exportReq, CancellationToken cancellationToken = default);
    }
}
