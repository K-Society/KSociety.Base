// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentExport<in TExportReq, TExportRes> : IAgentExportBase<TExportReq, TExportRes>, IAgentExportAsync<TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {

    }
}