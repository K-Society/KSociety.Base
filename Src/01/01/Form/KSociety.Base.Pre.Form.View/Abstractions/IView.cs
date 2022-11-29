using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    public interface IView<T, in TList>
        : IDataGridView<T>, IViewBase<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}