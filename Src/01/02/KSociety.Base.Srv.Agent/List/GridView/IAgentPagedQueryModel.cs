using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Srv.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.List.GridView;

public interface IAgentPagedQueryModel<T, TList>
    where T : IObject
    where TList : InfraSub.Shared.Interface.IList<T>
{
    TList LoadPagedRecords(PagedRequest pagedRequest, CancellationToken cancellationToken = default);

    ValueTask<TList> LoadPagedRecordsAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default);
}