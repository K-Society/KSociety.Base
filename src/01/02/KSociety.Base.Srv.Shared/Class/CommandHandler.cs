using System;
using Autofac;
using KSociety.Base.App.Shared;
using KSociety.Base.Srv.Shared.Interface;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Srv.Shared.Class
{
    public class CommandHandler : ICommandHandler
    {
        #region [ExecuteListWithResponse<TRequest, TRequestList, TResponse>]

        /// <summary>
        /// The ExecuteListWithResponse.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TRequestList"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="loggerFactory"></param>
        /// <param name="componentContext"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public TResponse ExecuteListWithResponse<TRequest, TRequestList, TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext, TRequestList request)
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandler>();
            return ExecutingListWithResponse<TRequest, TRequestList, TResponse>(logger, componentContext, request);
        }

        private static TResponse ExecutingListWithResponse<TRequest, TRequestList, TResponse>(
            ILogger logger,
            IComponentContext componentContext,
            TRequestList request
        )
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : IResponse, new()
        {
            var openType = typeof(IRequestListHandlerWithResponse<,,>); // Generic open type.
            var type = openType.MakeGenericType(typeof(TRequest), typeof(TRequestList),
                typeof(TResponse)); // Type is your runtime type.

            if (!componentContext.IsRegistered(type))
            {
                logger.LogError("Execute: {0} - Request: {1} Response: {2} not registered!", type.FullName,
                    typeof(TRequest).FullName, typeof(TResponse).FullName);
                return new TResponse();
            }

            try
            {
                var requestHandler = componentContext.Resolve(type);
                var methodInfo = type.GetMethod("Execute");
                //logger.LogTrace("Execute: " + openType.Name + " - " + " Request: " + typeof(TRequest).FullName + " Response: " + typeof(TResponse).FullName);
                return (TResponse)methodInfo?.Invoke(requestHandler, new[] {(object)request});
            }
            catch (Exception ex)
            {
                throw LogExceptionManager.LogException(logger, ex);
            }
        }

        #endregion

        #region [Execute<TRequest, TResponse>]

        /// <summary>
        /// The ExecuteWithResponse.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="loggerFactory"></param>
        /// <param name="componentContext"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public TResponse ExecuteWithResponse<TRequest, TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new()
            where TResponse : IResponse, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandler>();
            return ExecutingWithResponse<TRequest, TResponse>(logger, componentContext, request);
        }

        private static TResponse ExecutingWithResponse<TRequest, TResponse>(
            ILogger logger,
            IComponentContext componentContext,
            TRequest request
        )
            where TRequest : IRequest, new()
            where TResponse : IResponse, new()
        {
            var openType = typeof(IRequestHandlerWithResponse<,>); // Generic open type.
            var type = openType.MakeGenericType(typeof(TRequest), typeof(TResponse)); // Type is your runtime type.

            if (!componentContext.IsRegistered(type))
            {
                logger.LogError("Execute: {0} - Request: {1} Response: {2} not registered!", type.FullName,
                    typeof(TRequest).FullName, typeof(TResponse).FullName);
                return new TResponse();
            }

            try
            {
                var requestHandler = componentContext.Resolve(type);
                var methodInfo = type.GetMethod("Execute");
                //logger.LogTrace("Execute: " + openType.Name + " - " + " Request: " + typeof(TRequest).FullName + " Response: " + typeof(TResponse).FullName);
                return (TResponse)methodInfo?.Invoke(requestHandler, new[] {(object)request});
            }
            catch (Exception ex)
            {
                throw LogExceptionManager.LogException(logger, ex);
            }
        }

        #endregion

        #region [Execute]

        /// <summary>
        /// The Execute.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <param name="componentContext"></param>
        /// <param name="serviceName"></param>
        public void Execute(ILoggerFactory loggerFactory, IComponentContext componentContext, string serviceName)
        {
            var logger = loggerFactory.CreateLogger<CommandHandler>();
            Executing(logger, componentContext, serviceName);
        }

        private static void Executing(ILogger logger, IComponentContext componentContext, string serviceName)
        {
            try
            {
                //var openType = typeof(IRequestHandler); // Generic open type.
                var type = typeof(IRequestHandler); // Type is your runtime type.
                if (!componentContext.IsRegisteredWithName<IRequestHandler>(serviceName))
                {
                    logger.LogError("Execute: {0} - {1} not registered!", type.FullName, serviceName);
                    return;
                }

                try
                {
                    var requestHandler = componentContext.ResolveNamed<IRequestHandler>(serviceName);
                    var methodInfo = type.GetMethod("Execute");
                    //logger.LogTrace("Execute: " + type.Name + " - " + serviceName + " Response: void");
                    if (methodInfo != null) methodInfo.Invoke(requestHandler, null);
                }
                catch (Exception ex)
                {
                    throw LogExceptionManager.LogException(logger, ex);
                }
            }
            catch (Exception eex)
            {
                if (eex.InnerException != null) { throw eex.InnerException; }

                throw;
            }
        }

        #endregion

        #region [Execute<TRequest>]

        /// <summary>
        /// The Execute<TRequest>.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="loggerFactory"></param>
        /// <param name="componentContext"></param>
        /// <param name="request"></param>
        public void Execute<TRequest>(ILoggerFactory loggerFactory, IComponentContext componentContext,
            TRequest request)
            where TRequest : IRequest, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandler>();
            Executing(logger, componentContext, request);
        }

        private static void Executing<TRequest>(ILogger logger, IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new()
        {
            try
            {
                var openType = typeof(IRequestHandler<>); // Generic open type.
                var type = openType.MakeGenericType(typeof(TRequest)); // Type is your runtime type.
                if (!componentContext.IsRegistered(type))
                {
                    logger.LogError("Execute: {0} - Request: {1} not registered!", type.FullName,
                        typeof(TRequest).FullName);
                    return;
                }

                try
                {
                    var requestHandler = componentContext.Resolve(type);
                    var methodInfo = type.GetMethod("Execute");
                    //logger.LogTrace("Execute: " + openType.Name + " - " + " Request: " + typeof(TRequest).FullName + " Response: void");
                    if (methodInfo != null) methodInfo.Invoke(requestHandler, new[] {(object)request});
                }
                catch (Exception ex)
                {
                    throw LogExceptionManager.LogException(logger, ex);
                }
            }
            catch (Exception eex)
            {
                if (eex.InnerException != null) { throw eex.InnerException; }

                throw;
            }
        }

        #endregion

        #region [ExecuteWithResponse<TResponse>]

        /// <summary>
        /// The ExecuteWithResponse<TResponse>.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="loggerFactory"></param>
        /// <param name="componentContext"></param>
        /// <returns></returns>
        public TResponse ExecuteWithResponse<TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext)
            where TResponse : IResponse, new()
        {
            var logger = loggerFactory.CreateLogger<CommandHandler>();
            return ExecutingWithResponse<TResponse>(logger, componentContext);
        }

        private static TResponse ExecutingWithResponse<TResponse>(ILogger logger, IComponentContext componentContext)
            where TResponse : IResponse, new()
        {
            try
            {
                var openType = typeof(IRequestHandlerWithResponse<>); // Generic open type.
                var type = openType.MakeGenericType(typeof(TResponse)); // Type is your runtime type.
                if (!componentContext.IsRegistered(type))
                {
                    logger.LogError("Execute: {0} - Response: {1} not registered!", type.FullName,
                        typeof(TResponse).FullName);
                    return new TResponse();
                }

                try
                {
                    var requestHandler = componentContext.Resolve(type);
                    var methodInfo = type.GetMethod("Execute");
                    //logger.LogTrace("Execute: " + openType.Name + " - " + " Request: " + typeof(TResponse).FullName + " Response: void");
                    return (TResponse)methodInfo?.Invoke(requestHandler, null);
                }
                catch (Exception ex)
                {
                    throw LogExceptionManager.LogException(logger, ex);
                }
            }
            catch (Exception eex)
            {
                if (eex.InnerException != null) { throw eex.InnerException; }

                throw;
            }
        }

        #endregion
    }
}