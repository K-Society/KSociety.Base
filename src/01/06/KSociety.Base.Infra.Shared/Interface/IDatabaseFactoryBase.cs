using System;
using Microsoft.EntityFrameworkCore;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IDatabaseFactoryBase<out TContext> : IDisposable
        where TContext : DbContext
    {
        TContext? Get();
    }
}