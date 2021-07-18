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

        #region [GetConnectionString]

        public string GetConnectionString()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();
            return ConnectionString(callOptions);
        }

        public string GetConnectionString(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            return ConnectionString(callOptions);
        }

        private string ConnectionString(CallOptions callOptions)
        {
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

        public async ValueTask<string> GetConnectionStringAsync()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();
            return await ConnectionStringAsync(callOptions);
        }

        public async ValueTask<string> GetConnectionStringAsync(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            return await ConnectionStringAsync(callOptions);
        }

        private async ValueTask<string> ConnectionStringAsync(CallOptions callOptions)
        {
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

        #endregion

        #region [EnsureCreated]

        public bool EnsureCreated()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();
            return Create(callOptions);
        }

        public bool EnsureCreated(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            return Create(callOptions);
        }

        private bool Create(CallOptions callOptions)
        {
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

        public async ValueTask<bool> EnsureCreatedAsync()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();
            return await CreateAsync(callOptions);
        }

        public async ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            return await CreateAsync(callOptions);    
        }

        private async ValueTask<bool> CreateAsync(CallOptions callOptions)
        {
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

        #endregion

        #region [EnsureDeleted]

        public bool EnsureDeleted()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions  = new CallOptions();

            return Deleted(callOptions);
        }

        public bool EnsureDeleted(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions  = new CallOptions().WithCancellationToken(cancellationToken);
            
            return Deleted(callOptions);
        }

        private bool Deleted(CallOptions callOptions)
        {
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

        public async ValueTask<bool> EnsureDeletedAsync()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();

            return await DeletedAsync(callOptions);
        }

        public async ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            return await DeletedAsync(callOptions);
        }

        private async ValueTask<bool> DeletedAsync(CallOptions callOptions)
        {
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

        #endregion

        #region [Migration]

        public void Migration()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();

            Migration(callOptions);
        }

        public void Migration(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            Migration(callOptions);
        }

        private void Migration(CallOptions callOptions)
        {
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

        public async ValueTask MigrationAsync()
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions();

            await MigrationAsync(callOptions);
        }

        public async ValueTask MigrationAsync(CancellationToken cancellationToken)
        {
            Logger.LogTrace(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);

            await MigrationAsync(callOptions);
        }

        private async ValueTask MigrationAsync(CallOptions callOptions)
        {
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

        #endregion
    }
}
