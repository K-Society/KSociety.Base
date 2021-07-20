using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.List.GridView
{
    public interface IAgentQueryModel<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadAllRecords();
        TList LoadAllRecords(CancellationToken cancellationToken);
        ValueTask<TList> LoadAllRecordsAsync();
        ValueTask<TList> LoadAllRecordsAsync(CancellationToken cancellationToken);
    }
}
