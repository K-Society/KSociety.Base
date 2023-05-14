using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.List
{
    public interface IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {
        ValueTask<TList> LoadAllRecordsAsync(CancellationToken cancellationToken = default);
    }
}
