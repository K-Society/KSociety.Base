using System;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    public interface IAppDtoObject<out TRemove, out TAddReq, out TUpdateReq, out TCopyReq> : IObject
        where TRemove : class
        where TAddReq : class
        where TUpdateReq : class
        where TCopyReq : class
    {
        Guid Id { get; set; }

        TRemove GetRemoveReq();

        TAddReq GetAddReq();

        TUpdateReq GetUpdateReq();

        TCopyReq GetCopyReq();
    }
}
