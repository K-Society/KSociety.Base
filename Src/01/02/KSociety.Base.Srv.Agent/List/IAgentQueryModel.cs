using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.List
{
    public interface IAgentQueryModel<T, TList>
        where T : IObject
        where TList : InfraSub.Shared.Interface.IList<T>
    {
        TList LoadAllRecords(CancellationToken cancellationToken = default);

        ValueTask<TList> LoadAllRecordsAsync(CancellationToken cancellationToken = default);
    }
}
