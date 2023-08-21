namespace KSociety.Base.Srv.Agent.Control
{
    using Grpc.Core;
    using KSociety.Base.Srv.Contract.Control;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class DatabaseControl : Connection, IAgentDatabaseControl
    {
        public DatabaseControl(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {

        }

        public string GetConnectionString(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControl>();

                    return client.GetConnectionString(this.ConnectionOptions(cancellationToken)).Result;
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return String.Empty;
        }

        public async ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControlAsync>();
                    var result = await client.GetConnectionStringAsync(this.ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);
                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return String.Empty;
        }

        public bool EnsureCreated(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureCreated(this.ConnectionOptions(cancellationToken)).Result;
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureCreatedAsync(this.ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public bool EnsureDeleted(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureDeleted(this.ConnectionOptions(cancellationToken)).Result;
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureDeletedAsync(this.ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public void Migration(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControl>();

                    client.Migration(this.ConnectionOptions(cancellationToken));
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }
        }

        public async ValueTask MigrationAsync(CancellationToken cancellationToken = default)
        {
            this.Logger?.LogTrace("{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel.CreateGrpcService<IDatabaseControlAsync>();

                    await client.MigrationAsync(this.ConnectionOptions(cancellationToken)).ConfigureAwait(false);
                }
            }
            catch (RpcException rex)
            {
                this.Logger?.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1} - {2}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }
        }
    }
}
