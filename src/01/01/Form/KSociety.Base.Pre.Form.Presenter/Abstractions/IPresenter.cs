namespace KSociety.Base.Pre.Form.Presenter.Abstractions
{
    using InfraSub.Shared.Interface;
    using KSociety.Base.Pre.Form.View.Abstractions;

    public interface IPresenter<out TView, T, TList> : IPresenterBase<TView, T, TList>
        where T : IObject
        where TList : IList<T>
        where TView : IView<T, TList>
    {

    }
}
