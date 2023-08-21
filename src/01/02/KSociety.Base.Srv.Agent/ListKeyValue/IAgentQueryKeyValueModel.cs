// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    using Dto;

    public interface IAgentQueryKeyValueModel<TKey, TValue, TList> : IAgentQueryKeyValueModelBase<TKey, TValue, TList>, IAgentQueryKeyValueModelAsync<TKey, TValue, TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {

    }
}
