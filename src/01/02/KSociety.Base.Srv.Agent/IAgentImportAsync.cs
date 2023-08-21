// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentImportAsync<in TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {
        ValueTask<TImportRes> ImportDataAsync(TImportReq importReq, CancellationToken cancellationToken = default);
    }
}
