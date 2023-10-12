// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using InfraSub.Shared.Interface;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentQueryBaseModel<T>
        where T : IObject
    {
        T Find(CancellationToken cancellationToken = default);

        ValueTask<T> FindAsync(CancellationToken cancellationToken = default);
    }
}
