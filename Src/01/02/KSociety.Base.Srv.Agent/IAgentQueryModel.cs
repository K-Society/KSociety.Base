using KSociety.Base.InfraSub.Shared.Interface;

namespace KSociety.Base.Srv.Agent
{
    public interface IAgentQueryModel<in TObject, T> : 
        IAgentQueryModelBase<TObject, T>, 
        IAgentQueryModelAsync<TObject, T>
        where TObject : IIdObject
        where T : IObject
    {

    }
}