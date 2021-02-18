//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace KSociety.Base.Srv.Shared.Class
//{
//    public class DispatcherMediatorService
//    {
//        private readonly IMediator _mediator;

//        public DispatcherMediatorService(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        public void Dispatch(INotification notifyMessage)
//        {
//            _mediator.Send(notifyMessage);
//        }

//        public async ValueTask DispatchAsync(INotification notifyMessage, CancellationToken cancellationToken = default)
//        {
//            await _mediator.Send(notifyMessage, cancellationToken).ConfigureAwait(false);
//        }
//    }
//}
