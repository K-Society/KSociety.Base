using KSociety.Base.App.Shared.Dto.Res.Control;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace KSociety.Base.Srv.Contract.Control
{
    [Service]
    public interface IDatabaseControl
    {
        [Operation]
        EnsureCreated EnsureCreated();

        [Operation]
        EnsureCreated EnsureCreated(CallContext context);

        [Operation]
        EnsureDeleted EnsureDeleted();

        [Operation]
        EnsureDeleted EnsureDeleted(CallContext context);

        [Operation]
        void Migration();

        [Operation]
        void Migration(CallContext context);

        [Operation]
        ConnectionString GetConnectionString();

        [Operation]
        ConnectionString GetConnectionString(CallContext context);
    }
}
