namespace KSociety.Base.Srv.Agent
{
    public interface IAgentConfiguration
    {
        string ConnectionUrl { get; }

        bool DebugFlag { get; }
    }
}
