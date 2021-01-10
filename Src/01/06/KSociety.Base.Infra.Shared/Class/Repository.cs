using CsvHelper.Configuration;
using KSociety.Base.Infra.Shared.Csv;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Class
{
    public abstract class Repository<TContext, TEntity, TClassMap> : RepositoryBase<TContext, TEntity>, IRepository<TEntity>
        where TContext : DatabaseContext
        where TEntity : class
        where TClassMap : ClassMap<TEntity>
    {
        protected Repository(ILoggerFactory loggerFactory, IDatabaseFactory<TContext> databaseFactory)
            : base(loggerFactory, databaseFactory)
        {

        }

        public void ExportCsv(string fileName)
        {
            //Logger.LogTrace("RepositoryBase ExportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            WriteCsvClassMap<TEntity, TClassMap>.Export(LoggerFactory, fileName, FindAll());
        }

        public async ValueTask ExportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            //Logger.LogTrace("RepositoryBase ExportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            await WriteCsvClassMap<TEntity, TClassMap>.ExportAsync(LoggerFactory, fileName, FindAll()).ConfigureAwait(false);
        }
    }
}
