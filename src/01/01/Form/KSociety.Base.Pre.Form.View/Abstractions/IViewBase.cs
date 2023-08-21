namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using InfraSub.Shared.Interface;

    public interface IViewBase<T, in TList>
        : IDataGridViewBase, IListView<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}
