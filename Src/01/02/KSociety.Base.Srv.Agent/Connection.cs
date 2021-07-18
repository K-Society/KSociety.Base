using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using ProtoBuf.Grpc.Client;

namespace KSociety.Base.Srv.Agent
{
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
                    return GrpcChannel.ForAddress(_agentConfiguration.ConnectionUrl /*, new GrpcChannelOptions { HttpClient = httpClient }*/);
                }
                catch (RpcException rex)
                {
                    Logger.LogError(rex.Source + " - " + rex.Status.StatusCode + " " + rex.Status.Detail + " " + rex.Message + " " + rex.StackTrace);
                }

                return null;
            }
        }

        //public CallOptions CallOptions { get; set; }

        //public CallContext CallContext { get; set; }

        public  bool DebugFlag { get; }

        private readonly IAgentConfiguration _agentConfiguration;

        protected Connection(IAgentConfiguration agentConfiguration, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<Connection>();
            _agentConfiguration = agentConfiguration;

            //CallOptions = new CallOptions();
            //CallContext = new CallContext(CallOptions);

            DebugFlag = agentConfiguration.DebugFlag;

            if (DebugFlag)
            {
                Logger.LogTrace("Grpc Agent Connection for: " + _agentConfiguration.ConnectionUrl);
            }
        }
    }
}
