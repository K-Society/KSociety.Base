// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus
{
    /// <summary>
    /// The QueueDeclare parameters.
    /// </summary>
    public interface IQueueDeclareParameters
    {
        /// <summary>
        /// The Queue durable flag property.
        /// </summary>
        bool QueueDurable { get; set; }

        /// <summary>
        /// The Queue exclusive flag property.
        /// </summary>
        bool QueueExclusive { get; set; }

        /// <summary>
        /// The Queue auto delete flag property.
        /// </summary>
        bool QueueAutoDelete { get; set; }
    }
}