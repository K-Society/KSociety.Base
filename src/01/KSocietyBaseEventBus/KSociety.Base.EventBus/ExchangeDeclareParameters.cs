// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus
{
    ///<inheritdoc/>
    public class ExchangeDeclareParameters : IExchangeDeclareParameters
    {
        ///<inheritdoc/>
        public string BrokerName { get; set; }

        ///<inheritdoc/>
        public string ExchangeType { get; set; }

        ///<inheritdoc/>
        public string ExchangeName
        {
            get
            {
                return this.BrokerName + "_" + this.ExchangeType;
            }
        }

        ///<inheritdoc/>
        public bool ExchangeDurable { get; set; }

        ///<inheritdoc/>
        public bool ExchangeAutoDelete { get; set; }

        public ExchangeDeclareParameters() { }

        public ExchangeDeclareParameters(string brokerName, ExchangeType exchangeType, bool exchangeDurable = false,
            bool exchangeAutoDelete = false)
        {
            this.BrokerName = brokerName;
            this.ExchangeType = exchangeType.ToString().ToLower();
            this.ExchangeDurable = exchangeDurable;
            this.ExchangeAutoDelete = exchangeAutoDelete;
        }
    }
}
