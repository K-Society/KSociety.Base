namespace KSociety.Base.Srv.Agent
{
    /// <summary>
    /// The Agent Configuration.
    /// </summary>
    public interface IAgentConfiguration
    {
        /// <summary>
        /// The connection url.
        /// </summary>
        string ConnectionUrl { get; }

        /// <summary>
        /// The debug flag.
        /// </summary>
        bool DebugFlag { get; }
    }
}
