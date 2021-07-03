using KSociety.Base.Infra.Shared.Class;

namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IDatabaseConfiguration
    {
        DatabaseEngine DatabaseEngine { get; }
        string ConnectionString { get; }
        bool Logging { get; }
        string MigrationsAssembly { get; }
        string Version { get; }
    }
}
