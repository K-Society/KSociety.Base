// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    /// <summary>
    /// The INotifierMediatorService interface.
    /// </summary>
    public interface INotifierMediatorService
    {
        /// <summary>
        /// Notify the message.
        /// </summary>
        /// <param name="notifyMessage"><see cref="INotification"/></param>
        void Notify(INotification notifyMessage);

        /// <summary>
        /// Notify the message asynchronously.
        /// </summary>
        /// <param name="notifyMessage"><see cref="INotification"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns></returns>
        ValueTask NotifyAsync(INotification notifyMessage, CancellationToken cancellationToken = default);
    }
}