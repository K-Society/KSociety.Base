// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent.List.GridView
{
    using InfraSub.Shared.Interface;
    using System.Threading;

    public interface IAgentQueryModelBase<T, out TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadAllRecords(CancellationToken cancellationToken = default);
    }
}
