using KSociety.Base.InfraSub.Shared.Class;
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

namespace KSociety.Base.EventBusRabbitMQ
{
    public class DefaultRabbitMqPersistentConnection
        : DisposableObject, IRabbitMqPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMqPersistentConnection>? _logger;
        private IConnection? _connection;

        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);
        private readonly CancellationTokenSource _closeTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _closeToken;

        #region [Constructor]

        private DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _closeToken = _closeTokenSource.Token;
        }

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMqPersistentConnection>? logger = default)
        :this(connectionFactory)
        {
            logger ??= new NullLogger<DefaultRabbitMqPersistentConnection>();
            _logger = logger;
        }

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        :this(connectionFactory)
        {
            _logger = loggerFactory.CreateLogger<DefaultRabbitMqPersistentConnection>() ??
                      throw new ArgumentNullException(nameof(loggerFactory));
        }

        #endregion

        public bool IsConnected => _connection is {IsOpen: true} && !Disposed;

        public IModel? CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection?.CreateModel();
        }

        protected override void DisposeManagedResources()
        {
            try
            {
                _connection?.Dispose();
            }
            catch (IOException ex)
            {
                _logger?.LogError(ex, "DisposeManagedResources: ");
            }
        }

        public async ValueTask<bool> TryConnectAsync()
        {
            bool output;
            await _connectionLock.WaitAsync(_closeToken).ConfigureAwait(false);

            try
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .Or<Exception>()
                    .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            _logger?.LogWarning(ex, "TryConnectAsync: ");
                        });


                await policy.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        _logger?.LogInformation("RabbitMQ CreateConnection.");
                        //_logger.LogTrace("RabbitMQ CreateConnection StackTrace: {0}", System.Environment.StackTrace);
                        _connection = _connectionFactory
                            .CreateConnection(); //ToDo
                    }, _closeToken);

                });

                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdownAsync;
                    _connection.CallbackException += OnCallbackExceptionAsync;
                    _connection.ConnectionBlocked += OnConnectionBlockedAsync;

                    _logger?.LogInformation(
                        $"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
                    output = true;
                }
                else
                {
                    _logger?.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    output = false;
                }
            }
            finally
            {
                _connectionLock.Release();
            }

            return output;
        }

        private async void OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
        {
            if (Disposed) return;

            _logger?.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            await TryConnectAsync().ConfigureAwait(false);
        }

        private async void OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
        {
            if (Disposed) return;

            _logger?.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            await TryConnectAsync().ConfigureAwait(false);
        }

        private async void OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
        {
            if (Disposed) return;

            _logger?.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            await TryConnectAsync().ConfigureAwait(false);
        }
    }
}