using CsvHelper.Configuration;
using KSociety.Base.Infra.Shared.Csv;
using KSociety.Base.Infra.Shared.Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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

        public bool ImportCsv(string fileName)
        {
            //Logger.LogTrace("RepositoryBase ImportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                var result = ReadCsv<TEntity>.Import(LoggerFactory, fileName);
                if (!result.Any()) return false;
                DeleteRange(FindAll());
                
                AddRange(result);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public bool ImportCsv(byte[] byteArray)
        {
            //Logger.LogTrace("RepositoryBase ImportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                var result = ReadCsv<TEntity>.Import(LoggerFactory, byteArray);
                if (!result.Any()) { return false; }
                DeleteRange(FindAll());

                AddRange(result);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ImportCsvAsync(string fileName)
        {
            Logger.LogTrace("RepositoryBase ImportCsvAsync: " + fileName + " " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                //var result = ReadCsv<TEntity>.ImportAsync(LoggerFactory, fileName, cancellationToken).ConfigureAwait(false);

                DeleteRange(FindAll());
                //Logger.LogTrace("DeleteRange OK.");
                await foreach (var entity in ReadCsv<TEntity>.ImportAsync(LoggerFactory, fileName).ConfigureAwait(false).ConfigureAwait(false))
                {
                    var result = await AddAsync(entity).ConfigureAwait(false);
                    //Logger.LogTrace("AddAsync OK. " + result.Entity.GetType().Name);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ImportCsvAsync(string fileName, CancellationToken cancellationToken)
        {
            Logger.LogTrace("RepositoryBase ImportCsvAsync: " + fileName + " " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                //var result = ReadCsv<TEntity>.ImportAsync(LoggerFactory, fileName, cancellationToken).ConfigureAwait(false);

                DeleteRange(FindAll());
                //Logger.LogTrace("DeleteRange OK.");
                await foreach (var entity in ReadCsv<TEntity>.ImportAsync(LoggerFactory, fileName, cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    var result = await AddAsync(entity, cancellationToken).ConfigureAwait(false);
                    //Logger.LogTrace("AddAsync OK. " + result.Entity.GetType().Name);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ImportCsvAsync(byte[] byteArray)
        {
            Logger.LogTrace("RepositoryBase ImportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                //var result = ReadCsv<TEntity>.ImportAsync(LoggerFactory, byteArray, cancellationToken).ConfigureAwait(false);

                DeleteRange(FindAll());

                await foreach (var entity in ReadCsv<TEntity>.ImportAsync(LoggerFactory, byteArray).ConfigureAwait(false).ConfigureAwait(false))
                {
                    await AddAsync(entity).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ImportCsvAsync(byte[] byteArray, CancellationToken cancellationToken)
        {
            Logger.LogTrace("RepositoryBase ImportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                //var result = ReadCsv<TEntity>.ImportAsync(LoggerFactory, byteArray, cancellationToken).ConfigureAwait(false);

                DeleteRange(FindAll());

                await foreach (var entity in ReadCsv<TEntity>.ImportAsync(LoggerFactory, byteArray, cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await AddAsync(entity, cancellationToken).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public bool ExportCsv(string fileName)
        {
            //Logger.LogTrace("RepositoryBase ExportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                WriteCsvClassMap<TEntity, TClassMap>.Export(LoggerFactory, fileName, FindAll());

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex,GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ExportCsvAsync(string fileName)
        {
            //Logger.LogTrace("RepositoryBase ExportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                await WriteCsvClassMap<TEntity, TClassMap>.ExportAsync(LoggerFactory, fileName, FindAll())
                    .ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ExportCsvAsync(string fileName, CancellationToken cancellationToken)
        {
            //Logger.LogTrace("RepositoryBase ExportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                await WriteCsvClassMap<TEntity, TClassMap>.ExportAsync(LoggerFactory, fileName, FindAll())
                    .ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " + ex.Message);
            }

            return false;
        }
    }
}
