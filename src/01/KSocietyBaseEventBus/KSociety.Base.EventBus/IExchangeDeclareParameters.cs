﻿namespace KSociety.Base.EventBus
{
    /// <summary>
    /// The ExchangeDeclare parameters.
    /// </summary>
    public interface IExchangeDeclareParameters
    {
        /// <summary>
        /// The Broker name property.
        /// </summary>
        string BrokerName { get; set; }

        /// <summary>
        /// The Exchange type property.
        /// </summary>
        string ExchangeType { get; set; }

        /// <summary>
        /// The Exchange name property.
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// The Exchange durable flag property.
        /// </summary>
        bool ExchangeDurable { get; set; }

        /// <summary>
        /// The Exchange auto delete flag property.
        /// </summary>
        bool ExchangeAutoDelete { get; set; }
    }
}