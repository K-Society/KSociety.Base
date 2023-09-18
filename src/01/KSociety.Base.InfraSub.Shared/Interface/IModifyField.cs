// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Interface
{
    using System;

    public interface IModifyField
    {
        Guid Id { get; set; }
        string? FieldName { get; set; }
        string? Value { get; set; }
    }
}
