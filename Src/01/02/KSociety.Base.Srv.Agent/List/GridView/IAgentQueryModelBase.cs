using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;

namespace KSociety.Base.Srv.Agent.List.GridView
{
    public interface IAgentQueryModelBase<T, out TList>
        where T : IObject
        where TList : IList<T>
    {
        TList LoadAllRecords(CancellationToken cancellationToken = default);
    }
}