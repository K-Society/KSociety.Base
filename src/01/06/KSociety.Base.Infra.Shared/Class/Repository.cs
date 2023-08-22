// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class
{
    using CsvHelper.Configuration;
    using Interface;
    using KSociety.Base.InfraSub.Shared.Class.Csv;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    ///<inheritdoc cref="IRepository{TEntity}"/>
    public abstract class Repository<TContext, TEntity, TClassMap> : RepositoryBase<TContext, TEntity>,
        IRepository<TEntity>
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
                var result = ReadCsv<TEntity>.Import(this.LoggerFactory, fileName);
                if (!result.Any()) {return false;}
                this.DeleteRange(this.FindAll());

                this.AddRange(result);

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " +
                    ex.Message);
            }

            return false;
        }

        public bool ImportCsv(byte[] byteArray)
        {
            //Logger.LogTrace("RepositoryBase ImportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                var result = ReadCsv<TEntity>.Import(this.LoggerFactory, byteArray);
                if (!result.Any()) {return false;}
                this.DeleteRange(this.FindAll());

                this.AddRange(result);

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " +
                    ex.Message);
            }

            return false;
        }

        public async ValueTask<bool> ImportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            this.Logger.LogTrace("RepositoryBase ImportCsvAsync: {0} {1}.{2}", fileName, this.GetType().FullName,
                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                //var result = ReadCsv<TEntity>.ImportAsync(LoggerFactory, fileName, cancellationToken).ConfigureAwait(false);

                this.DeleteRange(this.FindAll());
                //Logger.LogTrace("DeleteRange OK.");
                await foreach (var entity in ReadCsv<TEntity>.ImportAsync(this.LoggerFactory, fileName, cancellationToken)
                                   .ConfigureAwait(false).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    var result = await this.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                    //Logger.LogTrace("AddAsync OK. " + result.Entity.GetType().Name);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return false;
        }

        public async ValueTask<bool> ImportCsvAsync(byte[] byteArray, CancellationToken cancellationToken = default)
        {
            this.Logger.LogTrace("RepositoryBase ImportCsvAsync: {0}.{1}", this.GetType().FullName,
                System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                //var result = ReadCsv<TEntity>.ImportAsync(LoggerFactory, byteArray, cancellationToken).ConfigureAwait(false);

                this.DeleteRange(this.FindAll());

                await foreach (var entity in ReadCsv<TEntity>.ImportAsync(this.LoggerFactory, byteArray, cancellationToken)
                                   .ConfigureAwait(false).WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    await this.AddAsync(entity, cancellationToken).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return false;
        }

        public bool ExportCsv(string fileName)
        {
            //Logger.LogTrace("RepositoryBase ExportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                WriteCsvClassMap<TEntity, TClassMap>.Export(this.LoggerFactory, fileName, this.FindAll());

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return false;
        }

        public async ValueTask<bool> ExportCsvAsync(string fileName, CancellationToken cancellationToken = default)
        {
            //Logger.LogTrace("RepositoryBase ExportCsvAsync: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                await WriteCsvClassMap<TEntity, TClassMap>.ExportAsync(this.LoggerFactory, fileName, this.FindAll())
                    .ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "{0}.{1}", this.GetType().FullName,
                    System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            }

            return false;
        }
    }
}
