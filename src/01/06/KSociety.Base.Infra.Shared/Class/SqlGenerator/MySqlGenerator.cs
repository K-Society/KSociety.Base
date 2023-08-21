namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.EntityFrameworkCore.Migrations.Operations;
    using Microsoft.EntityFrameworkCore.Update;
    using Microsoft.Extensions.Logging;
    using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
    using Pomelo.EntityFrameworkCore.MySql.Migrations;
    using System;

    public class MySqlGenerator : MySqlMigrationsSqlGenerator
    {
        private readonly ILogger<MySqlGenerator> _logger;

        //It must be public
#if NET6_0_OR_GREATER
        public MySqlGenerator(
            ILoggerFactory loggerFactory,
            MigrationsSqlGeneratorDependencies dependencies,
            ICommandBatchPreparer commandBatchPreparer,
            IMySqlOptions options)
            : base(dependencies, commandBatchPreparer, options)
        {
            this._logger = loggerFactory.CreateLogger<MySqlGenerator>();
            this._logger.LogTrace("MySqlGenerator");
        }
#endif

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
