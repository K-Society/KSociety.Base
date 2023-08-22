// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent.List.GridView
{
    using InfraSub.Shared.Interface;
    using Dto;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentPagedQueryModel<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadPagedRecords(PagedRequest pagedRequest, CancellationToken cancellationToken = default);

        ValueTask<TList> LoadPagedRecordsAsync(PagedRequest pagedRequest,
            CancellationToken cancellationToken = default);
    }
}
