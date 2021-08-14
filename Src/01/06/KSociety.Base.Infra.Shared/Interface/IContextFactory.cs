using KSociety.Base.Infra.Shared.Class;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IContextFactory<out TContext> where TContext : DatabaseContext
    {
        TContext CreateDbContext(string[] args);
    }
}