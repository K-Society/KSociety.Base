// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Shared
{
    /// <summary>
    /// The IRequestListHandlerWithResponse interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    /// <typeparam name="TRequestList">A type that inherits from the <see cref="IAppList{T}"/> interface.</typeparam>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestListHandlerWithResponse<in TRequest, in TRequestList, out TResponse>
        where TRequest : IRequest
        where TRequestList : IAppList<TRequest>
        where TResponse : class, IResponse
    {
        TResponse Execute(TRequestList request);
    }

    /// <summary>
    /// The IRequestHandlerWithResponse interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestHandlerWithResponse<in TRequest, out TResponse>
        where TRequest : IRequest
        where TResponse : class, IResponse
    {
        TResponse Execute(TRequest request);
    }

    /// <summary>
    /// The IRequestHandler interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    public interface IRequestHandler<in TRequest>
        where TRequest : IRequest
    {
        void Execute(TRequest request);
    }

    /// <summary>
    /// The IRequestHandlerWithResponse interface.
    /// </summary>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestHandlerWithResponse<out TResponse>
        where TResponse : class, IResponse
    {
        TResponse Execute();
    }

    /// <summary>
    /// The IRequestHandler interface.
    /// </summary>
    public interface IRequestHandler
    {
        void Execute();
    }
}
