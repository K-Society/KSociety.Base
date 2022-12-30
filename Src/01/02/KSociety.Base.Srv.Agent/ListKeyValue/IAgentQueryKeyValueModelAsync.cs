using KSociety.Base.Srv.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    public interface IAgentQueryKeyValueModelAsync<TList, TKey, TValue>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        ValueTask<TList> LoadDataAsync(CancellationToken cancellationToken = default);
    }
}
