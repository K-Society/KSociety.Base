using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public class AgentCommandAsync<TCommandAsync, TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
        : Connection, IAgentCommandAsync<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
        where TCommandAsync : class, ICommandAsync<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
        where TAddReq : class
        where TAddRes : class
        where TUpdateReq : class
        where TUpdateRes : class
        where TCopyReq : class
        where TCopyRes : class
        where TModifyFieldReq : class
        where TModifyFieldRes : class
        where TRemoveReq : class
        where TRemoveRes : class
    {
        public AgentCommandAsync(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        : base(agentConfiguration, loggerFactory)
        {

        }

        public virtual async ValueTask<TAddRes> AddAsync(TAddReq addItem, CancellationToken cancellationToken = default)
        {
            TAddRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TCommandAsync>();

                    var result = await client.AddAsync(addItem, ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken = default)
        {
            TUpdateRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TCommandAsync>();

                    var result = await client.UpdateAsync(updateItem, ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual async ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken = default)
        {
            TCopyRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TCommandAsync>();

                    var result = await client.CopyAsync(copyItem, ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual async ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default)
        {
            TModifyFieldRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TCommandAsync>();

                    var result = await client.ModifyFieldAsync(modifyFieldItem, ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual async ValueTask<TRemoveRes> RemoveAsync(TRemoveReq removeItem, CancellationToken cancellationToken = default)
        {
            TRemoveRes output = default;
            try
            {
                using (Channel)
                {
                    var client = Channel.CreateGrpcService<TCommandAsync>();

                    var result = await client.RemoveAsync(removeItem, ConnectionOptions(cancellationToken))
                        .ConfigureAwait(false);

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }
    }
}
