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
    public class DatabaseControl : Connection
    {
        public DatabaseControl(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
            : base (agentConfiguration, loggerFactory)
        {

        }

        public string GetConnectionString(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.GetConnectionString(CallContext).Result;
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
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();
                    var result = await client.GetConnectionStringAsync(CallContext).ConfigureAwait(false);
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

        public bool EnsureCreate(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureCreated(CallContext).Result;
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

        public async ValueTask<bool> EnsureCreateAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureCreatedAsync(CallContext).ConfigureAwait(false);

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
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    return client.EnsureDeleted(CallContext).Result;
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
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    var result = await client.EnsureDeletedAsync(CallContext).ConfigureAwait(false);

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
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControl>();

                    client.Migration(CallContext);
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
            CallOptions = CallOptions.WithCancellationToken(cancellationToken);
            CallContext = new CallContext(CallOptions, CallContextFlags.IgnoreStreamTermination);
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<IDatabaseControlAsync>();

                    await client.MigrationAsync(CallContext).ConfigureAwait(false);
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
