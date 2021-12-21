namespace KSociety.Base.Infra.Shared.Class
{
    public class DatabaseOptions
    {
        public DatabaseEngine DatabaseEngine { get; set; }
        public string ConnectionString { get; set; }
        public bool Logging { get; set; }
        public string MigrationsAssembly { get; set; }
        public bool LazyLoading { get; set; }

        public DatabaseOptions()
        {

        }
    }
}
