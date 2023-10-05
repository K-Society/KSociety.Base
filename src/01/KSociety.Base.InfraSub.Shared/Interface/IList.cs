// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System.Collections.Generic;

    #if NETSTANDARD2_0

    public interface IList<T> where T : IObject
    {
        List<T> List { get; set; }
        int Count { get; }
    }

    #elif NETSTANDARD2_1

    public interface IList<T> where T : IObject
    {
        List<T> List { get; set; }
        int Count { get; }
    }

    #endif
}
