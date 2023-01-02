using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Srv.Agent.List
{
    public interface IAgentQueryModel<T, TList> : IAgentQueryModelBase<T, TList>, IAgentQueryModelAsync<T, TList>
        where T : IObject
        where TList : IList<T>
    {

    }
}