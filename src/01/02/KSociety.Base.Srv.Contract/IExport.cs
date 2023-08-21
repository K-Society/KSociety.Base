// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;

    [Service]
    public interface IExport<in TExportReq, out TExportRes>
        where TExportReq : class
        where TExportRes : class
    {
        [Operation]
        TExportRes ExportData(TExportReq exportReq, CallContext context = default);
    }
}
