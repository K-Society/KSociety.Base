// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Shared
{
    using System.Collections.Generic;
    using ProtoBuf;

    /// <inheritdoc/>
    [ProtoContract]
    public class AppList<T> : IAppList<T> where T : IRequest
    {
        /// <inheritdoc/>
        [ProtoMember(1)]
        public List<T> List
        {
            get;
            set;
        }
    }
}