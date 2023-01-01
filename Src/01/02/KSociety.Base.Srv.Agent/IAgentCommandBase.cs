﻿using System.Threading;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentCommandBase<
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
        TAddRes Add(TAddReq addItem, CancellationToken cancellationToken = default);
        TUpdateRes Update(TUpdateReq updateItem, CancellationToken cancellationToken = default);
        TCopyRes Copy(TCopyReq copyItem, CancellationToken cancellationToken = default);
        TModifyFieldRes ModifyField(TModifyFieldReq modifyFieldItem, CancellationToken cancellationToken = default);
        TRemoveRes Remove(TRemoveReq removeItem, CancellationToken cancellationToken = default);
    }
}