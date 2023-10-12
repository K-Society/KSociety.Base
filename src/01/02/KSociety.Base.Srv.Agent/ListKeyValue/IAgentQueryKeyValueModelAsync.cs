// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    using Dto;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentQueryKeyValueModelAsync<TKey, TValue, TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        ValueTask<TList> LoadDataAsync(CancellationToken cancellationToken = default);
    }
}
