using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.App.Shared
{
    public interface IRequestListHandlerWithResponseAsync<in TRequest, in TRequestList, TResponse>
        where TRequest : IRequest
        where TRequestList : IAppList<TRequest>
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync(TRequestList request, CancellationToken cancellationToken = default);
    }

    public interface IRequestHandlerWithResponseAsync<in TRequest, TResponse>
        where TRequest : IRequest
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IRequestHandlerAsync<in TRequest>
        where TRequest : IRequest
    {
        ValueTask ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IRequestHandlerWithResponseAsync<TResponse>
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public interface IRequestHandlerAsync
    {
        ValueTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
