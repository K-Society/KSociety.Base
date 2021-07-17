using System;

namespace KSociety.Base.InfraSub.Shared.Interface
{
    /// <summary>
    /// The IAppDtoObject interface.
    /// </summary>
    /// <typeparam name="TRemove"></typeparam>
    /// <typeparam name="TAddReq"></typeparam>
    /// <typeparam name="TUpdateReq"></typeparam>
    /// <typeparam name="TCopyReq"></typeparam>
    public interface IAppDtoObject<out TRemove, out TAddReq, out TUpdateReq, out TCopyReq> : IObject
        where TRemove : class
        where TAddReq : class
        where TUpdateReq : class
        where TCopyReq : class
    {
        /// <value>Gets or sets the id.</value>
        Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TRemove GetRemoveReq();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TAddReq GetAddReq();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TUpdateReq GetUpdateReq();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TCopyReq GetCopyReq();
    }
}
