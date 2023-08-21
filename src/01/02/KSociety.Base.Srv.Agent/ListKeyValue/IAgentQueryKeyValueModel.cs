namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    using Dto;

    public interface IAgentQueryKeyValueModel<TKey, TValue, TList> : IAgentQueryKeyValueModelBase<TKey, TValue, TList>, IAgentQueryKeyValueModelAsync<TKey, TValue, TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {

    }
}
