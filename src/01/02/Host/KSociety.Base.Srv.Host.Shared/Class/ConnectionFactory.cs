// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class ConnectionFactory
    {
        public string MqHostName { get; set; } = "localhost";
        public string MqUserName { get; set; } = "KSociety";
        public string MqPassword { get; set; } = "KSociety";

        public ConnectionFactory()
        {

        }
    }
}