namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using InfraSub.Shared.Interface;

    public interface IView<T, in TList>
        : IDataGridView<T>, IViewBase<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}
