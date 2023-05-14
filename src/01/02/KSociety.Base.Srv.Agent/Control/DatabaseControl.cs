﻿using Grpc.Core;
using KSociety.Base.Srv.Contract.Control;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent.Control
{
    public class DatabaseControl : Connection, IAgentDatabaseControl
    {
        public DatabaseControl(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base(agentConfiguration, loggerFactory)
        {

        }

        public string GetConnectionString(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.GetConnectionString(ConnectionOptions(cancellationToken)).Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return string.Empty;
        }

        public async ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();
                    var result = await client.GetConnectionStringAsync(ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);
                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return string.Empty;
        }

        public bool EnsureCreated(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureCreated(ConnectionOptions(cancellationToken)).Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureCreatedAsync(ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public bool EnsureDeleted(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureDeleted(ConnectionOptions(cancellationToken)).Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureDeletedAsync(ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }

            return false;
        }

        public void Migration(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    client.Migration(ConnectionOptions(cancellationToken));
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }
        }

        public async ValueTask MigrationAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace("{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    await client.MigrationAsync(ConnectionOptions(cancellationToken)).ConfigureAwait(false);
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "{0} - {1} - {2}", rex.Source, rex.Status.StatusCode, rex.Status.Detail);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1} - {2}", GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name, ex.Source);
            }
        }
    }
}