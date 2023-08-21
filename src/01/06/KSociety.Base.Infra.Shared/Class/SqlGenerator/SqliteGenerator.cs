namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore.Migrations.Operations;
    using Microsoft.Extensions.Logging;
    using System;

    //No Abstract.
    public class SqliteGenerator : SqliteMigrationsSqlGenerator
    {
        private readonly ILogger<SqliteGenerator> _logger;

        //It must be public.
        public SqliteGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
            this._logger = loggerFactory.CreateLogger<SqliteGenerator>();
            this._logger.LogTrace("SqliteGenerator");
        }

        protected override void Generate(
            MigrationOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            try
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
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Generate: ");
            }
        }
    }
}
