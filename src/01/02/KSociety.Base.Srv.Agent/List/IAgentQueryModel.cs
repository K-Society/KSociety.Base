// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent.List
{
    using InfraSub.Shared.Interface;

    public interface IAgentQueryModel<T, TList> : IAgentQueryModelBase<T, TList>, IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {

    }
}
