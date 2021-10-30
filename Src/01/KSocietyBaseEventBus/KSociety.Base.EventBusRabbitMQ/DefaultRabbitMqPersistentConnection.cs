using System;
using System.IO;
using System.Net.Sockets;
using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Threading.Tasks;
using System.Threading;

namespace KSociety.Base.EventBusRabbitMQ
{
    public class DefaultRabbitMqPersistentConnection
       : DisposableObject, IRabbitMqPersistentConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<DefaultRabbitMqPersistentConnection> _logger;
        private IConnection _connection;

        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly CancellationTokenSource _closeTokenSource = new();
        private readonly CancellationToken _closeToken;

        public DefaultRabbitMqPersistentConnection(IConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = loggerFactory.CreateLogger<DefaultRabbitMqPersistentConnection>() ?? throw new ArgumentNullException(nameof(loggerFactory));

            _closeToken = _closeTokenSource.Token;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !Disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        protected override void DisposeManagedResources()
        {
            try
            {
                _connection.Dispose();
            }
            catch (IOException ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }

        //public bool TryConnect()
        //{
        //    _logger.LogInformation("RabbitMQ Client is trying to connect");
        //    try
        //    {
        //        lock (_syncRoot)
        //        {
        //            var policy = Policy.Handle<SocketException>()
        //                .Or<BrokerUnreachableException>()
        //                .Or<Exception>()
        //                .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        //                    (ex, time) =>
        //                            {
        //                                _logger.LogWarning(ex.Message + " - " + ex.StackTrace);
        //                            }
        //                );

        //            policy.Execute(() =>
        //            {
        //                _logger.LogInformation("RabbitMQ CreateConnection");
        //                _connection = _connectionFactory
        //                    .CreateConnection(); //ToDo
        //            });

        //            if (IsConnected)
        //            {
        //                _connection.ConnectionShutdown += OnConnectionShutdown;
        //                _connection.CallbackException += OnCallbackException;
        //                _connection.ConnectionBlocked += OnConnectionBlocked;

        //                _logger.LogInformation(
        //                    $"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
        //                return true;
        //            }

        //            _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("TryConnect: " + ex.Message + " - " + ex.StackTrace);
        //        return false;
        //    }
        //}

        public async ValueTask<bool> TryConnectAsync()
        {
            bool output;
            await _connectionLock.WaitAsync(_closeToken);

            try
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .Or<Exception>()
                    .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                                {
                                    _logger.LogWarning(ex, "TryConnectAsync: ");
                                });


                await policy.ExecuteAsync(async () =>
                {
                    await Task.Run(() =>
                    {
                        _logger.LogInformation("RabbitMQ CreateConnection.");
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

                    _logger.LogInformation(
                        $"RabbitMQ persistent connection acquired a connection {_connection.Endpoint.HostName} and is subscribed to failure events");
                    output = true;
                }
                else
                {
                    _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                    output = false;
                }
            }
            finally
            {
                _connectionLock.Release();
            }

            return output;
        }

        //private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        //{
        //    if (Disposed) return;

        //    _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
        //    TryConnect();
        //}

        //private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        //{
        //    if (Disposed) return;

        //    _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
        //    TryConnect();
        //}

        //private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        //{
        //    if (Disposed) return;

        //    _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
        //    TryConnect();
        //}

        private async void OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
        {
            if (Disposed) return;

            _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
            await TryConnectAsync().ConfigureAwait(false);
        }

        private async void OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
        {
            if (Disposed) return;

            _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
            await TryConnectAsync().ConfigureAwait(false);
        }

        private async void OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
        {
            if (Disposed) return;

            _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
            await TryConnectAsync().ConfigureAwait(false);
        }
    }
}
