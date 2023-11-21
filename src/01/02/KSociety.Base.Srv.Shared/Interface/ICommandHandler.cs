// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Shared.Interface
{
    using Autofac;
    using KSociety.Base.App.Shared;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The ICommandHandler interface.
    /// </summary>
    public interface ICommandHandler
    {
        #region [ExecuteListWithResponse<TRequest, TRequestList, TResponse>]

        /// <summary>
        /// ExecuteListWithResponse
        /// </summary>
        /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
        /// <typeparam name="TRequestList">A type that inherits from the <see cref="IAppList{T}"/> interface.</typeparam>
        /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        /// <param name="componentContext"><see cref="IComponentContext"/></param>
        /// <param name="request">A type that inherits from the <see cref="IRequest"/> interface.</param>
        /// <returns>A type that inherits from the <see cref="IResponse"/> interface.</returns>
        TResponse? ExecuteListWithResponse<TRequest, TRequestList, TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext, TRequestList request)
            where TRequest : IRequest, new()
            where TRequestList : IAppList<TRequest>, new()
            where TResponse : class, IResponse, new();

        #endregion

        #region [Execute<TRequest, TResponse>]

        /// <summary>
        /// ExecuteWithResponse
        /// </summary>
        /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
        /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        /// <param name="componentContext"><see cref="IComponentContext"/></param>
        /// <param name="request">A type that inherits from the <see cref="IRequest"/> interface.</param>
        /// <returns>A type that inherits from the <see cref="IResponse"/> interface.</returns>
        TResponse? ExecuteWithResponse<TRequest, TResponse>(ILoggerFactory loggerFactory,
            IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new()
            where TResponse : class, IResponse, new();

        #endregion

        #region [Execute]

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        /// <param name="componentContext"><see cref="IComponentContext"/></param>
        /// <param name="serviceName">Name of the service.</param>
        /// <example>
        /// <code>
        /// _commandHandler.Execute(_loggerFactory, _componentContext, "MigrationReqHdlr");
        /// </code>
        /// </example>
        void Execute(ILoggerFactory loggerFactory, IComponentContext componentContext, string serviceName);

        #endregion

        #region [Execute<TRequest>]

        /// <summary>
        /// Execute
        /// </summary>
        /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        /// <param name="componentContext"><see cref="IComponentContext"/></param>
        /// <param name="request">A type that inherits from the <see cref="IRequest"/> interface.</param>
        void Execute<TRequest>(ILoggerFactory loggerFactory, IComponentContext componentContext, TRequest request)
            where TRequest : IRequest, new();

        #endregion

        #region [ExecuteWithResponse<TResponse>]

        /// <summary>
        /// ExecuteWithResponse
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        /// <param name="componentContext"><see cref="IComponentContext"/></param>
        /// <returns>A type that inherits from the <see cref="IResponse"/> interface.</returns>
        TResponse? ExecuteWithResponse<TResponse>(ILoggerFactory loggerFactory, IComponentContext componentContext)
            where TResponse : class, IResponse, new();

        #endregion
    }
}
