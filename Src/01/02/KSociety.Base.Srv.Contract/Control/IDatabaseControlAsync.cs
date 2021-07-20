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
        ValueTask<EnsureCreated> EnsureCreatedAsync();

        [Operation]
        ValueTask<EnsureCreated> EnsureCreatedAsync(CallContext context);

        [Operation]
        ValueTask<EnsureDeleted> EnsureDeletedAsync();

        [Operation]
        ValueTask<EnsureDeleted> EnsureDeletedAsync(CallContext context);

        [Operation]
        ValueTask MigrationAsync();

        [Operation]
        ValueTask MigrationAsync(CallContext context);

        [Operation]
        ValueTask<ConnectionString> GetConnectionStringAsync();

        [Operation]
        ValueTask<ConnectionString> GetConnectionStringAsync(CallContext context);
    }
}
