using KSociety.Base.Srv.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    public interface IAgentQueryKeyValueModelAsync<TKey, TValue, TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        ValueTask<TList> LoadDataAsync(CancellationToken cancellationToken = default);
    }
}
