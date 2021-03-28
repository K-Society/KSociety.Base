using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Pre.Form.View.Abstractions
{
    public interface IViewReadOnly<T, in TList>
        : IViewBase<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}
