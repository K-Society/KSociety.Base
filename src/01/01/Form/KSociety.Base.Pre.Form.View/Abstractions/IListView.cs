namespace KSociety.Base.Pre.Form.View.Abstractions
{
    using InfraSub.Shared.Interface;

    public interface IListView<T, in TList> where T : IObject where TList : IList<T>
    {
        TList ListView { set; }
    }
}
