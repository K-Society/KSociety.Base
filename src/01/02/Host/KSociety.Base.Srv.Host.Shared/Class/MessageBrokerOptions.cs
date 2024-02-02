// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class MessageBrokerOptions
    {
        public ConnectionFactory ConnectionFactory { get; set; }
        public ExchangeDeclareParameters ExchangeDeclareParameters { get; set; }
        public QueueDeclareParameters QueueDeclareParameters { get; set; }

        public MessageBrokerOptions()
        {

        }
    }
}
