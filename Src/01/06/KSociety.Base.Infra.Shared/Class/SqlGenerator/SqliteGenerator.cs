using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

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
                    Generate(createViewOperation, builder);
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

        private void Generate(
            CreateViewOperation operation,
            MigrationCommandListBuilder builder)
        {
            try
            {
                var sqlHelper = Dependencies.SqlGenerationHelper;

                var assembly = AssemblyTool.GetAssemblyByName(operation.AssemblyName);

                string resourceName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(operation.ResourceSqlFileName));

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new(stream ?? throw new InvalidOperationException());
                string result = reader.ReadToEnd();

                _logger.LogDebug(result);

                builder.AppendLines(result)
                    .AppendLine(sqlHelper.StatementTerminator)
                    .EndCommand();
            }
            catch (Exception ex)
            {
                _logger.LogError("Generate " + ex.Message + " " + ex.StackTrace);
            }
        }
    }
}
