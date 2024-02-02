// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Domain.Shared.Class
{
    using System.Collections.Generic;
    using KSociety.Base.InfraSub.Shared.Class;
    using MediatR;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The abstract BaseEntity class.
    /// Contains all methods for adding a logger and events to the domain entity.
    /// </summary>
    /// <remarks>
    /// This class can add and remove events and add a logger or logger factory.
    /// </remarks>
    public abstract class BaseEntity : DisposableObject
    {
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => this._domainEvents?.AsReadOnly();

        public ILoggerFactory LoggerFactory;
        public ILogger Logger;

        /// <summary>
        /// Add the <see cref="ILoggerFactory"/> to the domain base entity and convert it to <see cref="ILogger"/>.
        /// </summary>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            this.LoggerFactory = loggerFactory;
            this.AddLogger(loggerFactory);
        }

        /// <summary>
        /// Add the <see cref="ILogger"/> to the domain base entity.
        /// </summary>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        public void AddLogger(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        /// <summary>
        /// Add the <see cref="ILogger"/> to the domain base entity.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/></param>
        public void AddLogger(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Add an event to the domain base entity.
        /// </summary>
        /// <param name="eventItem"><see cref="INotification"/></param>
        public void AddDomainEvent(INotification eventItem)
        {
            //_domainEvents ??= new List<INotification>();
            if (this._domainEvents is null)
            {
                this._domainEvents = new List<INotification>();
            }

            this._domainEvents.Add(eventItem);
        }

        /// <summary>
        /// Remove an event from the domain base entity.
        /// </summary>
        /// <param name="eventItem"><see cref="INotification"/></param>
        public void RemoveDomainEvent(INotification eventItem)
        {
            this._domainEvents?.Remove(eventItem);
        }

        /// <summary>
        /// Remove all events from the domain base entity.
        /// </summary>
        public void ClearDomainEvents()
        {
            this._domainEvents?.Clear();
        }
    }
}
