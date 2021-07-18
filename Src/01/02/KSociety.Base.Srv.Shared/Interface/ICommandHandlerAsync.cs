using System.Threading;
using System.Threading.Tasks;
using Autofac;
using KSociety.Base.App.Shared;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Shared.Interface
{
    public interface ICommandHandlerAsync
    {
        #region [ExecuteListWithResponseAsync<TRequest, TRequestList, TResponse>]

        ValueTask<TResponse> ExecuteListWithResponseAsync<TRequest, TRequestList, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequestList request)
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new();

        ValueTask<TResponse> ExecuteListWithResponseAsync<TRequest, TRequestList, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequestList request, CancellationToken cancellationToken)
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new();

        #endregion

        #region [Execute<TRequest, TResponse>]

        ValueTask<TResponse> ExecuteWithResponseAsync<TRequest, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new()
            where TResponse : IResponse, new();

        ValueTask<TResponse> ExecuteWithResponseAsync<TRequest, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest, new()
            where TResponse : IResponse, new();

        #endregion

        #region [Execute]

        ValueTask ExecuteAsync(ILoggerFactory loggerFactory, IComponentContext componentContext, string serviceName);

        ValueTask ExecuteAsync(ILoggerFactory loggerFactory, IComponentContext componentContext, string serviceName, CancellationToken cancellationToken);

        #endregion

        #region [Execute<TRequest>]

        ValueTask ExecuteAsync<TRequest>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new();

        ValueTask ExecuteAsync<TRequest>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest, new();

        #endregion

        #region [Execute<TResponse>]

        ValueTask<TResponse> ExecuteWithResponseAsync<TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext)
            where TResponse : IResponse, new();

        ValueTask<TResponse> ExecuteWithResponseAsync<TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext, CancellationToken cancellationToken)
            where TResponse : IResponse, new();

        #endregion
    }
}
