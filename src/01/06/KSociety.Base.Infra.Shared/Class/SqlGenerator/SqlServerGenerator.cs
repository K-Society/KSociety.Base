namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore.Migrations.Operations;
    using Microsoft.EntityFrameworkCore.Update;
    using Microsoft.Extensions.Logging;
    using System;

    //No Abstract.
    public class SqlServerGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly ILogger<SqlServerGenerator> _logger;
        //It must be public.
#if NET6_0_OR_GREATER
        public SqlServerGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            ICommandBatchPreparer commandBatchPreparer)
            : base(dependencies, commandBatchPreparer)
        {
            this._logger = loggerFactory.CreateLogger<SqlServerGenerator>();
            this._logger.LogTrace("SqlServerGenerator");
        }
#endif


        protected override void Generate(
            MigrationOperation operation,
            IModel? model,
            MigrationCommandListBuilder builder)
        {
            try
            {
                if (operation is CreateViewOperation createViewOperation)
                {
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
