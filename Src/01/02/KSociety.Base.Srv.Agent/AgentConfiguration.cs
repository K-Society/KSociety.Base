namespace KSociety.Base.Srv.Agent
{
    ///<inheritdoc/>
    public class AgentConfiguration : IAgentConfiguration
    {
        ///<inheritdoc/>
        public string ConnectionUrl { get; }

        ///<inheritdoc/>
        public bool DebugFlag { get; }

        public string Token { get; }

        public AgentConfiguration(string connectionUrl, bool debugFlag, string token = null)
        {
            ConnectionUrl = connectionUrl;
            DebugFlag = debugFlag;
            Token = token;
        }
    }
}