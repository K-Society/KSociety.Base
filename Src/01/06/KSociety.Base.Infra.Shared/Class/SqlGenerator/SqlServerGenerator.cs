using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    //No Abstract.
    public class SqlServerGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly ILogger<SqlServerGenerator> _logger;

        //It must be public.
        public SqlServerGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
            _logger = loggerFactory.CreateLogger<SqlServerGenerator>();
            _logger.LogTrace("SqlServerGenerator");
        }

        protected override void Generate(
            MigrationOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            try
            {
                if (operation is CreateViewOperation createViewOperation)
                {
                    SqlGeneratorHelper.Generate(_logger, createViewOperation, builder, Dependencies.SqlGenerationHelper);
                }
                else
                {
                    base.Generate(operation, model, builder);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate: ");
            }
        }
    }
}
