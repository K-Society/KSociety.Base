// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Shared.Class
{
    using System.Threading;
    using System.Threading.Tasks;
    using KSociety.Base.InfraSub.Shared.Interface;
    using MediatR;

    public class NotifierMediatorService : INotifierMediatorService
    {
        private readonly IMediator _mediator;

        public NotifierMediatorService(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public void Notify(INotification notifyMessage)
        {
            Task.Run(() => this._mediator.Publish(notifyMessage).ConfigureAwait(false));
        }

        public async ValueTask NotifyAsync(INotification notifyMessage, CancellationToken cancellationToken = default)
        {
            await this._mediator.Publish(notifyMessage, cancellationToken).ConfigureAwait(false);
        }
    }
}
