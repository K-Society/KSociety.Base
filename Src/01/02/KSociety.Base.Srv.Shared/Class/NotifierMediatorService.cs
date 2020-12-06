using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.InfraSub.Shared.Interface;
using MediatR;

namespace KSociety.Base.Srv.Shared.Class
{
    //public class NotifierMediatorService<TNotificationMessage> : INotifierMediatorService<TNotificationMessage> where TNotificationMessage : class, INotification
    public class NotifierMediatorService : INotifierMediatorService
    {
        private readonly IMediator _mediator;

        public NotifierMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        //public void Notify(TNotificationMessage notifyMessage)
        //{
        //    _mediator.Publish(notifyMessage);
        //}

        //public async Task NotifyAsync(TNotificationMessage notifyMessage, CancellationToken cancellationToken = default)
        //{
        //    await _mediator.Publish(notifyMessage, cancellationToken);
        //}

        public void Notify(INotification notifyMessage)
        {
            _mediator.Publish(notifyMessage);
        }

        public async ValueTask NotifyAsync(INotification notifyMessage, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(notifyMessage, cancellationToken).ConfigureAwait(false);
        }
    }
}
