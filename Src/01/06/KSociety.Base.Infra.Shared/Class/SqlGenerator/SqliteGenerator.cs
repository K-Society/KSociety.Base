using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    //No Abstract
    public class SqliteGenerator : SqliteMigrationsSqlGenerator
    {
        private readonly ILogger<SqliteGenerator> _logger;

        //It must be public
        public SqliteGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
            _logger = loggerFactory.CreateLogger<SqliteGenerator>();
            _logger.LogTrace("SqliteGenerator");
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
                    //Generate(createViewOperation, builder);
                    SqlGeneratorHelper.Generate(_logger, createViewOperation, builder, Dependencies.SqlGenerationHelper);
                }
                else
                {
                    base.Generate(operation, model, builder);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Generate " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
