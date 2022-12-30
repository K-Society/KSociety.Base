using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentQueryModel<T, in TObject> : IAgentQueryModelAsync<T, TObject>
        where T : IObject
        where TObject : IIdObject
    {
        T Find(TObject idObject, CancellationToken cancellationToken = default);
    }
}