using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations;
using System;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    public class MySqlGenerator : MySqlMigrationsSqlGenerator
    {
        private readonly ILogger<MySqlGenerator> _logger;

        //It must be public
        public MySqlGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations,
            IMySqlOptions options)
            : base(dependencies, migrationsAnnotations, options)
        {
            _logger = loggerFactory.CreateLogger<MySqlGenerator>();
            _logger.LogTrace("MySqlGenerator");
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
                _logger.LogError(ex, "Generate: ");
            }
        }
    }
}
