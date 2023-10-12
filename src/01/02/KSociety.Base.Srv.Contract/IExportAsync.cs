// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;
    using System.Threading.Tasks;

    [Service]
    public interface IExportAsync<in TExportReq, TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        [Operation]
        ValueTask<TExportRes?> ExportDataAsync(TExportReq exportReq, CallContext context = default);
    }
}
