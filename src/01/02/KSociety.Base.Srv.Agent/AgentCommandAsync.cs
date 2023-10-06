// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using Contract;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TCommandAsync>();
                    if (client != null)
                    {
                        var result = await client.AddAsync(addItem, this.ConnectionOptions(cancellationToken))
                            .ConfigureAwait(false);

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

        public virtual async ValueTask<TUpdateRes> UpdateAsync(TUpdateReq updateItem, CancellationToken cancellationToken = default)
        {
            TUpdateRes output = default;
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TCommandAsync>();
                    if (client != null)
                    {
                        var result = await client.UpdateAsync(updateItem, this.ConnectionOptions(cancellationToken))
                            .ConfigureAwait(false);

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

        public virtual async ValueTask<TCopyRes> CopyAsync(TCopyReq copyItem, CancellationToken cancellationToken = default)
        {
            TCopyRes output = default;
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TCommandAsync>();
                    if (client != null)
                    {
                        var result = await client.CopyAsync(copyItem, this.ConnectionOptions(cancellationToken))
                            .ConfigureAwait(false);

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

        public virtual async ValueTask<TModifyFieldRes> ModifyFieldAsync(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default)
        {
            TModifyFieldRes output = default;
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TCommandAsync>();
                    if (client != null)
                    {
                        var result = await client
                            .ModifyFieldAsync(modifyFieldItem, this.ConnectionOptions(cancellationToken))
                            .ConfigureAwait(false);

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

        public virtual async ValueTask<TRemoveRes> RemoveAsync(TRemoveReq removeItem, CancellationToken cancellationToken = default)
        {
            TRemoveRes output = default;
            try
            {
                using (this.Channel)
                {
                    var client = this.Channel?.CreateGrpcService<TCommandAsync>();
                    if (client != null)
                    {
                        var result = await client.RemoveAsync(removeItem, this.ConnectionOptions(cancellationToken))
                            .ConfigureAwait(false);

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
