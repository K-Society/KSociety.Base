using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    public interface IListView<T, in TList> where T : IObject where TList : IList<T>
    {
        TList ListView { set; }
    }
}
