// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Contract.Control
{
    using KSociety.Base.App.Utility.Dto.Res.Control;
    using System.Threading.Tasks;
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;

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