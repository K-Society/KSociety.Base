using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    public interface INotifierMediatorService //<in TNotificationMessage> where TNotificationMessage : class, INotification
    {
        //void Notify(TNotificationMessage notifyMessage);

        //Task NotifyAsync(TNotificationMessage notifyMessage, CancellationToken cancellationToken = default);

        void Notify(INotification notifyMessage);

        ValueTask NotifyAsync(INotification notifyMessage, CancellationToken cancellationToken = default);
    }
}
