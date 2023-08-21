// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Shared
{
    using System.Threading;
    using System.Threading.Tasks;

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
        ValueTask<TResponse> ExecuteAsync(TRequestList request, CancellationToken cancellationToken = default);
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
        ValueTask<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The IRequestHandlerAsync interface.
    /// </summary>
    /// <typeparam name="TRequest">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    public interface IRequestHandlerAsync<in TRequest>
        where TRequest : IRequest
    {
        ValueTask ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The IRequestHandlerWithResponseAsync interface.
    /// </summary>
    /// <typeparam name="TResponse">A type that inherits from the <see cref="IResponse"/> interface.</typeparam>
    public interface IRequestHandlerWithResponseAsync<TResponse>
        where TResponse : IResponse
    {
        ValueTask<TResponse> ExecuteAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// The IRequestHandlerAsync interface.
    /// </summary>
    public interface IRequestHandlerAsync
    {
        ValueTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}