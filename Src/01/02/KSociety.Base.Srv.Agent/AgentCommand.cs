using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Agent
{
    public class AgentCommand<TCommand, TCommandAsync, TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes> 
        : Connection, IAgentCommand<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
        where TCommand : class, ICommand<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
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
        public AgentCommand(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        :base(agentConfiguration, loggerFactory)
        {

        }

        public TAddRes Add(TAddReq addItem, CancellationToken cancellationToken = default)
        {
            TAddRes output = default;
            try
            {
                using (Channel)
                {

                    var client = Channel.CreateGrpcService<TCommand>();

                    var result = client.Add(addItem, ConnectionOptions(cancellationToken));

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public async ValueTask<TAddRes> AddAsync(TAddReq addItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public TUpdateRes Update(TUpdateReq updateItem, CancellationToken cancellationToken = default)
        {
            TUpdateRes output = default;
            try
            {
                using (Channel)
                {

                    var client = Channel.CreateGrpcService<TCommand>();

                    var result = client.Update(updateItem, ConnectionOptions(cancellationToken));

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public TCopyRes Copy(TCopyReq copyItem, CancellationToken cancellationToken = default)
        {
            TCopyRes output = default;
            try
            {
                using (Channel)
                {

                    var client = Channel.CreateGrpcService<TCommand>();

                    var result = client.Copy(copyItem, ConnectionOptions(cancellationToken));

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public async ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public TModifyFieldRes ModifyField(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default)
        {
            TModifyFieldRes output = default;
            try
            {
                using (Channel)
                {

                    var client = Channel.CreateGrpcService<TCommand>();

                    var result = client.ModifyField(modifyFieldItem, ConnectionOptions(cancellationToken));

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public async ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public TRemoveRes Remove(TRemoveReq removeItem, CancellationToken cancellationToken = default)
        {
            TRemoveRes output = default;
            try
            {
                using (Channel)
                {

                    var client = Channel.CreateGrpcService<TCommand>();

                    var result = client.Remove(removeItem, ConnectionOptions(cancellationToken));

                    output = result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }

        public async ValueTask<TRemoveRes> RemoveAsync(TRemoveReq removeItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + " - " + ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
            return output;
        }
    }
}
