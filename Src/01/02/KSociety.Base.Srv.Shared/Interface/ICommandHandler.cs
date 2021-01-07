using Autofac;
using KSociety.Base.App.Shared;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Shared.Interface
{
    public interface ICommandHandler
    {
        #region [ExecuteListWithResponse<TRequest, TRequestList, TResponse>]

        TResponse ExecuteListWithResponse<TRequest, TRequestList, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequestList request)
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new();

        #endregion

        #region [Execute<TRequest, TResponse>]

        TResponse ExecuteWithResponse<TRequest, TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new()
            where TResponse : IResponse, new();

        #endregion

        #region [Execute]

        void Execute(ILoggerFactory loggerFactory, IComponentContext componentContext, string serviceName);

        #endregion

        #region [Execute<TRequest>]

        void Execute<TRequest>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new();

        #endregion

        #region [ExecuteWithResponse<TResponse>]

        TResponse ExecuteWithResponse<TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext)
            where TResponse : IResponse, new();

        #endregion
    }
}
