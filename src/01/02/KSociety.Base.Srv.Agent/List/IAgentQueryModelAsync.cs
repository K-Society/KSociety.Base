// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent.List
{
    using InfraSub.Shared.Interface;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        ValueTask<TList> LoadAllRecordsAsync(CancellationToken cancellationToken = default);
    }
}
