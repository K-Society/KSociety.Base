namespace KSociety.Base.Srv.Agent.List.GridView
{
    using InfraSub.Shared.Interface;
    using Dto;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAgentPagedQueryModel<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadPagedRecords(PagedRequest pagedRequest, CancellationToken cancellationToken = default);

        ValueTask<TList> LoadPagedRecordsAsync(PagedRequest pagedRequest,
            CancellationToken cancellationToken = default);
    }
}
