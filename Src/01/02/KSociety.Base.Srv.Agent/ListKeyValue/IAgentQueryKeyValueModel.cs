using KSociety.Base.Srv.Dto;

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    public interface IAgentQueryKeyValueModel<TKey, TValue, TList> : IAgentQueryKeyValueModelBase<TKey, TValue, TList>, IAgentQueryKeyValueModelAsync<TKey, TValue, TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {

    }
}