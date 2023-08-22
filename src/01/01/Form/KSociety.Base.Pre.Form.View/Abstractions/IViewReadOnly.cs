// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using InfraSub.Shared.Interface;

    public interface IViewReadOnly<T, in TList>
        : IViewBase<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}
