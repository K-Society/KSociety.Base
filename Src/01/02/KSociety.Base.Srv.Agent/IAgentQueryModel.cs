using KSociety.Base.InfraSub.Shared.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent;

public interface IAgentQueryModel<T, in TObject>
    where T : IObject
    where TObject : IIdObject
{
    T Find(TObject idObject, CancellationToken cancellationToken = default);

    ValueTask<T> FindAsync(TObject idObject, CancellationToken cancellationToken = default);
}