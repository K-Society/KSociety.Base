namespace KSociety.Base.Srv.Agent.ListKeyValue
{
    using Dto;
    using System.Threading;

    public interface IAgentQueryKeyValueModelBase<TKey, TValue, out TList>
        where TList : ListKeyValuePair<TKey, TValue>
    {
        TList LoadData(CancellationToken cancellationToken = default);
    }
}
