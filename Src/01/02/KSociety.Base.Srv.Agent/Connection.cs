using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using System;
using System.Threading;

namespace KSociety.Base.Srv.Agent;

public class Connection
{
    protected readonly ILogger<Connection> Logger;

    public GrpcChannel Channel
    {
        get
        {
            try
            {
                GrpcClientFactory.AllowUnencryptedHttp2 = true;
                //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                //var httpClientHandler = new HttpClientHandler
                //{
                //    ServerCertificateCustomValidationCallback =
                //        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                //};
                //var httpClient = new HttpClient(httpClientHandler);
                return GrpcChannel.ForAddress(_agentConfiguration.ConnectionUrl , new GrpcChannelOptions
                {
                    MaxReceiveMessageSize = null, // 5 * 1024 * 1024, // 5 MB
                    MaxSendMessageSize = null // 2 * 1024 * 1024 // 2 MB
                       
                });
            }
            catch (RpcException rex)
            {
                Logger.LogError(rex, "Channel: ");
            }

            return null;
        }
    }

    public  bool DebugFlag { get; }

    private readonly IAgentConfiguration _agentConfiguration;

    protected Connection(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<Connection>();
        _agentConfiguration = agentConfiguration;

        DebugFlag = agentConfiguration.DebugFlag;

        if (DebugFlag)
        {
            Logger.LogTrace(@"Grpc Agent Connection for: {0}", _agentConfiguration.ConnectionUrl);
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
        WriteOptions writeOptions = null, ContextPropagationToken propagationToken = null, CallCredentials credentials = null)
    {
        var callOptions = new CallOptions(headers, deadline, cancellationToken, writeOptions, propagationToken, credentials);
        return new CallContext(callOptions, CallContextFlags.IgnoreStreamTermination);
    }
}