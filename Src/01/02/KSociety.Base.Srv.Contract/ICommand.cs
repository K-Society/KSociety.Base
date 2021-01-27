//using ProtoBuf.Grpc;
//using ProtoBuf.Grpc.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace KSociety.Base.Srv.Contract
//{
//    [Service]
//    public interface ICommand<
//        in TRemove,
//        in TAddReq,
//        TAddRes,
//        in TUpdateReq,
//        TUpdateRes,
//        in TCopyReq,
//        TCopyRes,
//        in TModifyField>
//        where TRemove : class
//        where TAddReq : class
//        where TAddRes : class
//        where TUpdateReq : class
//        where TUpdateRes : class
//        where TCopyReq : class
//        where TCopyRes : class
//        where TModifyField : class
//    {
//        [Operation]
//        TAddRes Add(TAddReq request, CallContext context = default);

//        [Operation]
//        TUpdateRes Update(TUpdateReq request, CallContext context = default);

//        [Operation]
//        TCopyRes Copy(TCopyReq request, CallContext context = default);

//        [Operation]
//        App.Dto.Res.ModifyField.Common.Tag ModifyField(TModifyField request, CallContext context = default);

//        [Operation]
//        App.Dto.Res.Remove.Common.Tag Remove(TRemove request, CallContext context = default);
//    }
//}
