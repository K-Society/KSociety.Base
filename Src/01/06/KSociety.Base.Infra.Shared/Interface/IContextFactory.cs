using KSociety.Base.Infra.Shared.Class;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IContextFactory<out TContext> where TContext : DatabaseContext
    {
        //string dbEngine, string connectionString, string migrationsAssembly
        //TContext CreateDbContext(string[] args);
        TContext CreateDbContext(string dbEngine, string connectionString, string migrationsAssembly);
    }
}