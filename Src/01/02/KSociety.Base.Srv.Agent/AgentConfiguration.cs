namespace KSociety.Base.Srv.Agent
{
    ///<inheritdoc/>
    public class AgentConfiguration : IAgentConfiguration
    {
        ///<inheritdoc/>
        public string ConnectionUrl { get; }

        ///<inheritdoc/>
        public bool DebugFlag { get; }

        public AgentConfiguration(string connectionUrl, bool debugFlag)
        {
            ConnectionUrl = connectionUrl;
            DebugFlag = debugFlag;
        }
    }
}
