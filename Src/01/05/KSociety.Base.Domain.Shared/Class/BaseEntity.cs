using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Domain.Shared.Class
{
    public abstract class BaseEntity
    {
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public ILoggerFactory LoggerFactory;
        public ILogger Logger;

        public void AddLoggerFactory(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            AddLogger(loggerFactory);
        }

        public void AddLogger(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(GetType());
        }

        public void AddLogger(ILogger logger)
        {
            Logger = logger;
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

    }
}
