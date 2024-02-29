// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBusRabbitMQ
{
    using Helper;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Polly;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class DefaultRabbitMqPersistentConnection
        : Disposable, IRabbitMqPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMqPersistentConnection> _logger;
        private IConnection _connection;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _closeTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _closeToken;

        #region [Constructor]

        private DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory)
        {
            this._connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this._closeToken = this._closeTokenSource.Token;
        }

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMqPersistentConnection> logger = default)
        :this(connectionFactory)
        {
            if (logger == null)
            {
                logger = new NullLogger<DefaultRabbitMqPersistentConnection>();
            }
            this._logger = logger;
        }

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        :this(connectionFactory)
        {
            this._logger = loggerFactory.CreateLogger<DefaultRabbitMqPersistentConnection>();
        }

        #endregion

        public bool IsConnected => this._connection?.IsOpen == true && !this.IsDisposed;

        public IModel CreateModel()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return this._connection?.CreateModel();
        }

        
        #region [Dispose]

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    this._connection?.Dispose();
                }
                catch (IOException ex)
                {
                    this._logger?.LogError(ex, "Dispose: ");
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        public bool TryConnect()
        {
            bool output;
            this._connectionLock.Wait(this._closeToken);

            try
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .Or<Exception>()
                    .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            this._logger?.LogWarning(ex, "TryConnectAsync: ");
                        });


                policy.Execute(() =>
                {
                    //await Task.Run(() =>
                    //{
                        //this._logger?.LogInformation("RabbitMQ CreateConnection.");
                        //_logger.LogTrace("RabbitMQ CreateConnection StackTrace: {0}", System.Environment.StackTrace);
                        this._connection = this._connectionFactory
                            .CreateConnection(); //ToDo
                    //}, this._closeToken);

                });

                if (this.IsConnected)
                {
                    if (this._connection != null)
                    {
                        this._connection.ConnectionShutdown += this.OnConnectionShutdown;
                        this._connection.CallbackException += this.OnCallbackException;
                        this._connection.ConnectionBlocked += this.OnConnectionBlocked;

                        //this._logger?.LogInformation($"RabbitMQ persistent connection acquired a connection {this._connection.Endpoint.HostName} and is subscribed to failure events");
                        output = true;
                    }
                    else
                    {
                        output = false;
                    }
                }
                else
                {
                    this._logger?.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    output = false;
                }
            }
            finally
            {
                this._connectionLock.Release();
            }

            return output;
        }

        public async ValueTask<bool> TryConnectAsync()
        {
            bool output;
            await this._connectionLock.WaitAsync(this._closeToken).ConfigureAwait(false);

            try
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .Or<Exception>()
                    .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            this._logger?.LogWarning(ex, "TryConnectAsync: ");
                        });


                await policy.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        //this._logger?.LogInformation("RabbitMQ CreateConnection.");
                        //_logger.LogTrace("RabbitMQ CreateConnection StackTrace: {0}", System.Environment.StackTrace);
                        this._connection = this._connectionFactory
                            .CreateConnection(); //ToDo
                    }, this._closeToken);

                }).ConfigureAwait(false);

                if (this.IsConnected)
                {
                    if (this._connection != null)
                    {
                        this._connection.ConnectionShutdown += this.OnConnectionShutdownAsync;
                        this._connection.CallbackException += this.OnCallbackExceptionAsync;
                        this._connection.ConnectionBlocked += this.OnConnectionBlockedAsync;

                        //this._logger?.LogInformation($"RabbitMQ persistent connection acquired a connection {this._connection.Endpoint.HostName} and is subscribed to failure events");
                        output = true;
                    }
                    else
                    {
                        output = false;
                    }
                }
                else
                {
                    this._logger?.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    output = false;
                }
            }
            finally
            {
                this._connectionLock.Release();
            }

            return output;
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            this.TryConnect();
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            this.TryConnect();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            this.TryConnect();
        }

        private async void OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            await this.TryConnectAsync().ConfigureAwait(false);
        }

        private async void OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            await this.TryConnectAsync().ConfigureAwait(false);
        }

        private async void OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            await this.TryConnectAsync().ConfigureAwait(false);
        }
    }
}
