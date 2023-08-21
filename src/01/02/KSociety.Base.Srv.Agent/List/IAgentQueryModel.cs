namespace KSociety.Base.Srv.Agent.List
{
    using InfraSub.Shared.Interface;

    public interface IAgentQueryModel<T, TList> : IAgentQueryModelBase<T, TList>, IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {

    }
}
