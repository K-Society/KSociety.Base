// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using KSociety.Base.Domain.Shared.Class;
    using MediatR;

    public static class MediatorExtension
    {
        public static void DispatchDomainEvents(this IMediator mediator, DatabaseContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var entityEntries = domainEntities.ToList();
            var domainEvents = entityEntries
                .SelectMany(x => x.Entity.DomainEvents ?? Enumerable.Empty<INotification>())
                .ToList();

            entityEntries
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                mediator.Publish(domainEvent); //.Wait();
            }
        }

        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DatabaseContext ctx,
            CancellationToken cancellationToken = default)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var entityEntries = domainEntities.ToList();
            var domainEvents = entityEntries
                .SelectMany(x => x.Entity.DomainEvents ?? Enumerable.Empty<INotification>())
                .ToList();

            entityEntries
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
                });

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
