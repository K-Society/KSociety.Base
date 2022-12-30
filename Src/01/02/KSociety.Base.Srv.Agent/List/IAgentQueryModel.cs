﻿using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;

namespace KSociety.Base.Srv.Agent.List
{
    public interface IAgentQueryModel<T, TList> : IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadAllRecords(CancellationToken cancellationToken = default);
    }
}