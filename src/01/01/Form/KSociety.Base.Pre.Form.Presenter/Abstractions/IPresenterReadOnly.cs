using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Pre.Form.View.Abstractions;

namespace KSociety.Base.Pre.Form.Presenter.Abstractions
{
    public interface IPresenterReadOnly<out TView, T, TList> : IPresenterBase<TView, T, TList>
        where T : IObject
        where TList : IList<T>
        where TView : IViewReadOnly<T, TList>
    {
    }
}