namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    using Dto;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentQueryKeyValueModelAsync<TKey, TValue, TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        ValueTask<TList> LoadDataAsync(CancellationToken cancellationToken = default);
    }
}
