namespace KSociety.Base.Infra.Shared.Interface
{
    using Microsoft.EntityFrameworkCore;

    public interface IDatabaseFactory<out TContext> : IDatabaseFactoryBase<TContext>
        where TContext : DbContext
    {

    }
}
