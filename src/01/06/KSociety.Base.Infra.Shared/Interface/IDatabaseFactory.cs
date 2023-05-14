using Microsoft.EntityFrameworkCore;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IDatabaseFactory<out TContext> : IDatabaseFactoryBase<TContext>
        where TContext : DbContext
    {

    }
}