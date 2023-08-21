namespace KSociety.Base.Srv.Agent.List.GridView
{
    using InfraSub.Shared.Interface;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        ValueTask<TList> LoadAllRecordsAsync(CancellationToken cancellationToken = default);
    }
}
