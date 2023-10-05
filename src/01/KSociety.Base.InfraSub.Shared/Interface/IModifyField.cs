// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System;

    #if NETSTANDARD2_0

    public interface IModifyField
    {
        Guid Id { get; set; }
        string FieldName { get; set; }
        string Value { get; set; }
    }

    #elif NETSTANDARD2_1

    public interface IModifyField
    {
        Guid Id { get; set; }
        string FieldName { get; set; }
        string Value { get; set; }
    }

    #endif
}
