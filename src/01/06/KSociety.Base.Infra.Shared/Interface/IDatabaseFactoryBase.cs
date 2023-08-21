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