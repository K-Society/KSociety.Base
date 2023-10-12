// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Shared
{
    using System.Collections.Generic;

    /// <summary>
    /// The IAppList interface.
    /// </summary>
    /// <typeparam name="T">A type that inherits from the <see cref="IRequest"/> interface.</typeparam>
    public interface IAppList<T> where T : IRequest
    {
        /// <value>Gets or sets the list of the <see cref="IRequest"/>.</value>
        List<T>? List { get; set; }
    }
}
