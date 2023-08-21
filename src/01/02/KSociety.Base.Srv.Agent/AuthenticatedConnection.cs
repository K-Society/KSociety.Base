// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Agent
{
    using Grpc.Core;
    using Grpc.Net.Client;
    using Microsoft.Extensions.Logging;
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Client;
    using System;
    using System.Threading;

    public class AuthenticatedConnection
    {
        protected readonly ILogger<Connection> Logger;

        public GrpcChannel Channel
        {
            get
            {
                try
                {
                    var credentials = CallCredentials.FromInterceptor((context, metadata) =>
                    {
                        if (!String.IsNullOrEmpty(this._agentConfiguration.Token))
                        {
                            metadata.Add("Authorization", $"Bearer {this._agentConfiguration.Token}");
                        }

                        return null;
                    });

                    GrpcClientFactory.AllowUnencryptedHttp2 = true;
                    //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                    //var httpClientHandler = new HttpClientHandler
                    //{
                    //    ServerCertificateCustomValidationCallback =
                    //        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    //};
                    //var httpClient = new HttpClient(httpClientHandler);
                    return GrpcChannel.ForAddress(this._agentConfiguration.ConnectionUrl, new GrpcChannelOptions
                    {
                        MaxReceiveMessageSize = null, // 5 * 1024 * 1024, // 5 MB
                        MaxSendMessageSize = null, // 2 * 1024 * 1024 // 2 MB

                        Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
                    });
                }
                catch (RpcException rex)
                {
                    this.Logger.LogError(rex, "Channel: ");
                }

                return null;
            }
        }

        public bool DebugFlag { get; }

        private readonly IAgentConfiguration _agentConfiguration;

        protected AuthenticatedConnection(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger<Connection>();
            this._agentConfiguration = agentConfiguration;

            this.DebugFlag = agentConfiguration.DebugFlag;

            if (this.DebugFlag)
            {
                this.Logger.LogTrace(@"Grpc Agent Connection for: {0}", this._agentConfiguration.ConnectionUrl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual CallContext ConnectionOptions(CancellationToken cancellationToken = default)
        {
            var callOptions = new CallOptions().WithCancellationToken(cancellationToken);
            return new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="deadline"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="writeOptions"></param>
        /// <param name="propagationToken"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        protected virtual CallContext ConnectionOptions(Metadata headers = null,
            DateTime? deadline = null, CancellationToken cancellationToken = default,
            WriteOptions writeOptions = null, ContextPropagationToken propagationToken = null,
            CallCredentials credentials = null)
        {
            var callOptions = new CallOptions(headers, deadline, cancellationToken, writeOptions, propagationToken,
                credentials);
            return new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
        }
    }
}
