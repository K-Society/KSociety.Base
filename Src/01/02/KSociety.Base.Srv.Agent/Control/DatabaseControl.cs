using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using KSociety.Base.Srv.Contract.Control;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

namespace KSociety.Base.Srv.Agent.Control
{
    public class DatabaseControl : Connection, IAgentDatabaseControl
    {
        public DatabaseControl(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base (agentConfiguration, loggerFactory)
        {

        }

        public string GetConnectionString(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.GetConnectionString(callContext).Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return string.Empty;
        }

        public async ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();
                    var result = await client.GetConnectionStringAsync(callContext).ConfigureAwait(false);
                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return string.Empty;
        }

        public bool EnsureCreated(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureCreated(callContext).Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return false;
        }

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureCreatedAsync(callContext).ConfigureAwait(false);

                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return false;
        }

        public bool EnsureDeleted(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureDeleted(callContext).Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return false;
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureDeletedAsync(callContext).ConfigureAwait(false);

                    return result.Result;
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }

            return false;
        }

        public void Migration(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    client.Migration(callContext);
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
        }

        public async ValueTask MigrationAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            var callContext = new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    await client.MigrationAsync(callContext).ConfigureAwait(false);
                }
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
