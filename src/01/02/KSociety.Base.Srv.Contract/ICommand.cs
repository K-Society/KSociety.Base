// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Contract
{
    using ProtoBuf.Grpc;
    using ProtoBuf.Grpc.Configuration;

    [Service]
    public interface ICommand<
        in TAddReq, out TAddRes,
        in TUpdateReq, out TUpdateRes,
        in TCopyReq, out TCopyRes,
        in TModifyFieldReq, out TModifyFieldRes,
        in TRemoveReq, out TRemoveRes>
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
    {
        [Operation]
        TAddRes Add(TAddReq request, CallContext context = default);

        [Operation]
        TUpdateRes Update(TUpdateReq request, CallContext context = default);

        [Operation]
        TCopyRes Copy(TCopyReq request, CallContext context = default);

        [Operation]
        TModifyFieldRes ModifyField(TModifyFieldReq request, CallContext context = default);

        [Operation]
        TRemoveRes Remove(TRemoveReq request, CallContext context = default);
    }
}
