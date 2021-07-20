using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.App.Shared
{
    /// <summary>
    /// The IRequestListHandlerWithResponseAsync interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    /// <typeparam name="TRequestList">A type that inherits from the <see cref="IAppList{T}"/> interface.</typeparam>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestListHandlerWithResponseAsync<in TRequest, in TRequestList, TResponse>
        where TRequest : IRequest
        where TRequestList : IAppList<TRequest>
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync(TRequestList request);
        ValueTask<TResponse> ExecuteAsync(TRequestList request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The IRequestHandlerWithResponseAsync interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestHandlerWithResponseAsync<in TRequest, TResponse>
        where TRequest : IRequest
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync(TRequest request);
        ValueTask<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The IRequestHandlerAsync interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    public interface IRequestHandlerAsync<in TRequest>
        where TRequest : IRequest
    {
        ValueTask ExecuteAsync(TRequest request);
        ValueTask ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The IRequestHandlerWithResponseAsync interface.
    /// </summary>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestHandlerWithResponseAsync<TResponse>
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync();
        ValueTask<TResponse> ExecuteAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// The IRequestHandlerAsync interface.
    /// </summary>
    public interface IRequestHandlerAsync
    {
        ValueTask ExecuteAsync();
        ValueTask ExecuteAsync(CancellationToken cancellationToken);
    }
}
