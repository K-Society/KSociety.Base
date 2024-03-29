// Copyright � K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Form.Presenter.Abstractions
{
    using InfraSub.Shared.Interface;
    using KSociety.Base.Pre.Form.View.Abstractions;

    public interface IPresenterBase<out TView, T, TList>
        where T : IObject
        where TList : IList<T>
        where TView : IViewBase<T, TList>
    {
        TView GetView();
    }
}
