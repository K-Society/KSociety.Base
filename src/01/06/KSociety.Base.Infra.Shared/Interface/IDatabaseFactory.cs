// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Interface
{
    using Microsoft.EntityFrameworkCore;

    public interface IDatabaseFactory<out TContext> : IDatabaseFactoryBase<TContext>
        where TContext : DbContext
    {

    }
}
