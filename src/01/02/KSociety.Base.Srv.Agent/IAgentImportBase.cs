// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using System.Threading;

    public interface IAgentImportBase<in TImportReq, out TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        TImportRes ImportData(TImportReq importReq, CancellationToken cancellationToken = default);
    }
}
