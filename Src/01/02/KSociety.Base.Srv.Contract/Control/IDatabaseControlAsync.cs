using System.Threading.Tasks;
using KSociety.Base.App.Shared.Dto.Res.Control;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace KSociety.Base.Srv.Contract.Control
{
    [Service]
    public interface IDatabaseControlAsync
    {
        [Operation]
        ValueTask<EnsureCreated> EnsureCreatedAsync(CallContext context = default);

        [Operation]
        ValueTask<EnsureDeleted> EnsureDeletedAsync(CallContext context = default);

        [Operation]
        ValueTask MigrationAsync(CallContext context = default);

        [Operation]
        ValueTask<ConnectionString> GetConnectionStringAsync(CallContext context = default);
    }
}