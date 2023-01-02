using KSociety.Base.Srv.Contract;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public class AgentCommand<TCommand, TCommandAsync, TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes> 
        : AgentCommandAsync<TCommandAsync, TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>, 
            IAgentCommand<TAddReq, TAddRes, TUpdateReq, TUpdateRes, TCopyReq, TCopyRes, TModifyFieldReq, TModifyFieldRes, TRemoveReq, TRemoveRes>
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

        public virtual TAddRes Add(TAddReq addItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TUpdateRes Update(TUpdateReq updateItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TCopyRes Copy(TCopyReq copyItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TModifyFieldRes ModifyField(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TRemoveRes Remove(TRemoveReq removeItem, CancellationToken cancellationToken = default)
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
                Logger.LogError(ex, "{0}.{1}", GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }
    }
}
