using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace KSociety.Base.InfraSub.Shared.Interface;

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