using KSociety.Base.Srv.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    public interface IAgentQueryKeyValueModel<TList, TKey, TValue>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        TList LoadData();
        TList LoadData(CancellationToken cancellationToken);

        ValueTask<TList> LoadDataAsync();
        ValueTask<TList> LoadDataAsync(CancellationToken cancellationToken);
    }
}
