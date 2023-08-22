// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Interface
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public interface IDatabaseFactoryBase<out TContext> : IDisposable
        where TContext : DbContext
    {
        TContext? Get();
    }
}
