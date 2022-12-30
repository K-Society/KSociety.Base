using KSociety.Base.Srv.Dto;
using System.Threading;

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    public interface IAgentQueryKeyValueModel<TList, TKey, TValue> : IAgentQueryKeyValueModelAsync<TList, TKey, TValue>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        TList LoadData(CancellationToken cancellationToken = default);
    }
}