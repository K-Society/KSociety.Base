﻿// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System;

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