using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentQueryModelBase<in TObject, out T>
        where TObject : IIdObject
        where T : IObject
    {
        T Find(TObject idObject, CancellationToken cancellationToken = default);
    }
}