using KSociety.Base.App.Utility.Dto.Res.Control;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace KSociety.Base.Srv.Contract.Control
{
    [Service]
    public interface IDatabaseControl
    {
        [Operation]
        EnsureCreated EnsureCreated(CallContext context = default);

        [Operation]
        EnsureDeleted EnsureDeleted(CallContext context = default);

        [Operation]
        void Migration(CallContext context = default);

        [Operation]
        ConnectionString GetConnectionString(CallContext context = default);
    }
}