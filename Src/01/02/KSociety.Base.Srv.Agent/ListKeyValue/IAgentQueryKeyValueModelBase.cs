using KSociety.Base.Srv.Dto;
using System.Threading;

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    public interface IAgentQueryKeyValueModelBase<TKey, TValue, out TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        TList LoadData(CancellationToken cancellationToken = default);
    }
}
