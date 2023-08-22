// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.EventBus.Abstractions.Handler
{
    using System.Threading.Tasks;

    public interface IDynamicIntegrationEventHandler
    {
        ValueTask Handle(dynamic eventData);
    }
}