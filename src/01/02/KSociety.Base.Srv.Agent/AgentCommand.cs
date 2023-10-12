// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using Contract;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;

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

        public virtual TAddRes? Add(TAddReq addItem, CancellationToken cancellationToken = default)
        {
            TAddRes? output = default;
            try
            {
                using (this.Channel)
                {

                    var client = this.Channel?.CreateGrpcService<TCommand>();
                    if (client != null)
                    {
                        var result = client.Add(addItem, this.ConnectionOptions(cancellationToken));

                        output = result;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TUpdateRes? Update(TUpdateReq updateItem, CancellationToken cancellationToken = default)
        {
            TUpdateRes? output = default;
            try
            {
                using (this.Channel)
                {

                    var client = this.Channel?.CreateGrpcService<TCommand>();
                    if (client != null)
                    {
                        var result = client.Update(updateItem, this.ConnectionOptions(cancellationToken));

                        output = result;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TCopyRes? Copy(TCopyReq copyItem, CancellationToken cancellationToken = default)
        {
            TCopyRes? output = default;
            try
            {
                using (this.Channel)
                {

                    var client = this.Channel?.CreateGrpcService<TCommand>();
                    if (client != null)
                    {
                        var result = client.Copy(copyItem, this.ConnectionOptions(cancellationToken));

                        output = result;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TModifyFieldRes ModifyField(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default)
        {
            TModifyFieldRes output = default;
            try
            {
                using (this.Channel)
                {

                    var client = this.Channel?.CreateGrpcService<TCommand>();
                    if (client != null)
                    {
                        var result = client.ModifyField(modifyFieldItem, this.ConnectionOptions(cancellationToken));

                        output = result;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }

        public virtual TRemoveRes Remove(TRemoveReq removeItem, CancellationToken cancellationToken = default)
        {
            TRemoveRes output = default;
            try
            {
                using (this.Channel)
                {

                    var client = this.Channel?.CreateGrpcService<TCommand>();
                    if (client != null)
                    {
                        var result = client.Remove(removeItem, this.ConnectionOptions(cancellationToken));

                        output = result;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "{0}.{1}", this.GetType().FullName, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }
            return output;
        }
    }
}
