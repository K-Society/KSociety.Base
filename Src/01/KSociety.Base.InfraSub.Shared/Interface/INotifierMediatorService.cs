using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    public interface INotifierMediatorService
    {
        void Notify(INotification notifyMessage);

        ValueTask NotifyAsync(INotification notifyMessage, CancellationToken cancellationToken = default);
    }
}
