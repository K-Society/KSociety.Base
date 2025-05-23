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

        public bool IsConnected => this._connection != null && this._connection.IsOpen && !this.IsDisposed;

        public async Task<IChannel> CreateModel()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return await this._connection?.CreateChannelAsync();
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

        public async ValueTask<bool> TryConnectAsync()
        {
            bool output;
            //await this._connectionLock.WaitAsync(this._closeToken).ConfigureAwait(false);

            //try
            //{
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
                await this.CreateConnection().ConfigureAwait(false);
                //await Task.Run(this.CreateConnection, this._closeToken).ConfigureAwait(false);
                //this._closeToken;
            }).ConfigureAwait(false);

            if (this.IsConnected)
            {
                if (this._connection != null)
                {
                    this._connection.ConnectionShutdownAsync += this.OnConnectionShutdownAsync;
                    this._connection.CallbackExceptionAsync += this.OnCallbackExceptionAsync;
                    this._connection.ConnectionBlockedAsync += this.OnConnectionBlockedAsync;

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
            //}
            //finally
            //{
            //    this._connectionLock.Release();
            //}

            return output;
        }

        private async ValueTask CreateConnection()
        {
            await this._connectionLock.WaitAsync(this._closeToken).ConfigureAwait(false);
            try
            {
                if (this._connection == null)
                {
                    this._connection = await this._connectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, "CreateConnection: ");
                throw;
            }
            finally
            {
                this._connectionLock.Release();
            }
        }

        private async Task OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            await this.TryConnectAsync().ConfigureAwait(false);
        }

        private async Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
        {
            if (this.IsDisposed)
            {
                return;
            }

            this._logger?.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            await this.TryConnectAsync().ConfigureAwait(false);
        }

        private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
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
