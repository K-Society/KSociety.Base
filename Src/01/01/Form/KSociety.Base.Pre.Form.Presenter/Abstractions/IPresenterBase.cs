using KSociety.Base.InfraSub.Shared.Interface;
using KSociety.Base.Pre.Form.View.Abstractions;

namespace KSociety.Base.Pre.Form.Presenter.Abstractions;

public interface IPresenterBase<out TView, T, TList>
    where T : IObject
    where TList : IList<T>
    where TView : IViewBase<T, TList>
{
    TView GetView();
}