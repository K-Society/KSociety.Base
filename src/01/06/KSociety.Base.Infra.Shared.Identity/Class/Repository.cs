// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Identity.Class
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CsvHelper.Configuration;
    using KSociety.Base.Infra.Shared.Interface;
    using KSociety.Base.InfraSub.Shared.Class.Csv;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public abstract class Repository<TContext, TEntity, TClassMap, TUser, TRole, TKey, TUserClaim, TUserRole,
            TUserLogin, TRoleClaim, TUserToken>
        : RepositoryBase<TContext, TEntity, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken>, IRepository<TEntity>
        where TContext : DatabaseContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TEntity : class
        where TClassMap : ClassMap<TEntity>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        protected Repository(ILoggerFactory loggerFactory, Interface.IDatabaseFactory<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
                databaseFactory)
            : base(loggerFactory, databaseFactory)
        {

        }

        public bool ImportCsv(string fileName)
        {
            //Logger.LogTrace("RepositoryBase ImportCsv: " + GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name);
            try
            {
                var result = ReadCsv<TEntity>.Import(this.LoggerFactory, fileName);
                if (!result.Any())
                {
                    return false;
                }

                this.DeleteRange(this.FindAll());

                this.AddRange(result);

                return true;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex,
                    this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " +
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
                if (result != null)
                {
                    if (!result.Any())
                    {
                        return false;
                    }

                    this.DeleteRange(this.FindAll());

                    this.AddRange(result);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex,
                    this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod()?.Name + ": " +
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
                                   .ConfigureAwait(false).ConfigureAwait(false))
                {
                    var unused = await this.AddAsync(entity, cancellationToken).ConfigureAwait(false);
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
                                   .ConfigureAwait(false).ConfigureAwait(false))
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
