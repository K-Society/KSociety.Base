// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

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

        string Token { get; }
    }
}