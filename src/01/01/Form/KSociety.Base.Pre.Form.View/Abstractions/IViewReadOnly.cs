namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using InfraSub.Shared.Interface;

    public interface IViewReadOnly<T, in TList>
        : IViewBase<T, TList>
        where T : IObject where TList : IList<T>
    {

    }
}
