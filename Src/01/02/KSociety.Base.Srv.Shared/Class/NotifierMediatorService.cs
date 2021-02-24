using System.Threading;
using System.Threading.Tasks;
using KSociety.Base.InfraSub.Shared.Interface;
using MediatR;

namespace KSociety.Base.Srv.Shared.Class
{
    public class NotifierMediatorService : INotifierMediatorService
    {
        private readonly IMediator _mediator;

        public NotifierMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Notify(INotification notifyMessage)
        {
            Task.Run(() => _mediator.Publish(notifyMessage).ConfigureAwait(false));
        }

        public async ValueTask NotifyAsync(INotification notifyMessage, CancellationToken cancellationToken = default)
        {
            await _mediator.Publish(notifyMessage, cancellationToken).ConfigureAwait(false);
        }
    }
}
