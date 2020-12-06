using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc;

namespace KSociety.Base.Srv.Agent
{
    public interface IConnection
    {
        GrpcChannel Channel { get; }

        CallOptions CallOptions { get; set; }

        CallContext CallContext { get; set; }

        bool DebugFlag { get; }
    }
}
