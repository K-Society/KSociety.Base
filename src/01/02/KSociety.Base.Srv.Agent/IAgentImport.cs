// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentImport<in TImportReq, TImportRes> : IAgentImportBase<TImportReq, TImportRes>, IAgentImportAsync<TImportReq, TImportRes>
        where TImportReq : class
        where TImportRes : class
    {

    }
}