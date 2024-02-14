// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Host.Shared.Class
{
    public class ExchangeDeclareParameters
    {
        public string BrokerName { get; set; }
        public EventBus.ExchangeType ExchangeType { get; set; } = EventBus.ExchangeType.Direct;
        public bool ExchangeDurable { get; set; } = false;
        public bool ExchangeAutoDelete { get; set; } = true;

        public ExchangeDeclareParameters()
        {

        }
    }
}
