using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    //No Abstract.
    public class NpgsqlGenerator : NpgsqlMigrationsSqlGenerator
    {
        private readonly ILogger<NpgsqlGenerator> _logger;

        //It must be public.
#if NET6_0_OR_GREATER
        public NpgsqlGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            INpgsqlSingletonOptions migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
            this._logger = loggerFactory.CreateLogger<NpgsqlGenerator>();
            this._logger.LogTrace("NpgsqlGenerator");
        }
#endif

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Just because")]
        protected override void Generate(
            MigrationOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            if (operation is CreateViewOperation createViewOperation)
            {
                //Generate(createViewOperation, builder);
                SqlGeneratorHelper.Generate(this._logger, createViewOperation, builder, this.Dependencies.SqlGenerationHelper);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }
    }
}
