﻿using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentQueryBaseModel<T>
        where T : IObject
    {
        T Find(CancellationToken cancellationToken = default);

        ValueTask<T> FindAsync(CancellationToken cancellationToken = default);
    }
}