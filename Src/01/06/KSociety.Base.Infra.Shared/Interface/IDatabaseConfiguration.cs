namespace KSociety.Base.Infra.Shared.Interface
{
    public interface IDatabaseConfiguration
    {
        string ConnectionString { get; }

        bool Logging { get; }

        string MigrationsAssembly { get; }
    }
}
