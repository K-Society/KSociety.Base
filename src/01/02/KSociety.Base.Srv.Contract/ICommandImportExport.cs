// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc.Configuration;

    [Service]
    public interface ICommandImportExport<
        in TAddReq, out TAddRes,
        in TUpdateReq, out TUpdateRes,
        in TCopyReq, out TCopyRes,
        in TModifyFieldReq, out TModifyFieldRes,
        in TRemoveReq, out TRemoveRes,
        in TImportReq, out TImportRes,
        in TExportReq, out TExportRes> : ICommand<
        TAddReq, TAddRes,
        TUpdateReq, TUpdateRes,
        TCopyReq, TCopyRes,
        TModifyFieldReq, TModifyFieldRes,
        TRemoveReq, TRemoveRes>, IImport<TImportReq, TImportRes>, IExport<TExportReq, TExportRes>
        where TAddReq : class
        where TAddRes : class
        where TUpdateReq : class
        where TUpdateRes : class
        where TCopyReq : class
        where TCopyRes : class
        where TModifyFieldReq : class
        where TModifyFieldRes : class
        where TRemoveReq : class
        where TRemoveRes : class
        where TImportReq : class
        where TImportRes : class
        where TExportReq : class
        where TExportRes : class
    {
    }
}
