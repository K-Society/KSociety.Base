namespace KSociety.Base.Srv.Agent
{
    using InfraSub.Shared.Interface;

    public interface IAgentQueryModel<in TObject, T> : 
        IAgentQueryModelBase<TObject, T>, 
        IAgentQueryModelAsync<TObject, T>
        where TObject : IIdObject
        where T : IObject
    {

    }
}
