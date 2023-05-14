using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    public interface IViewBase<T, in TList>
        : IDataGridViewBase, IListView<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}