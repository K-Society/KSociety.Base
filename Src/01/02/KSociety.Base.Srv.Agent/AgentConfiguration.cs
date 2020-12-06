namespace KSociety.Base.Srv.Agent
{
    public class AgentConfiguration : IAgentConfiguration
    {
        public string ConnectionUrl { get; }

        public bool DebugFlag { get; }

        public AgentConfiguration(string connectionUrl, bool debugFlag)
        {
            ConnectionUrl = connectionUrl;
            DebugFlag = debugFlag;
        }
    }
}
