using KSociety.Base.Infra.Abstraction.Class;

namespace KSociety.Base.Infra.Abstraction.Interface
{
    /// <summary>
    /// The Database configuration.
    /// </summary>
    public interface IDatabaseConfiguration
    {
        DatabaseEngine DatabaseEngine { get; }
        string ConnectionString { get; }
        bool Logging { get; }
        string MigrationsAssembly { get; }
        bool LazyLoading { get; }
        string ToString();
    }
}