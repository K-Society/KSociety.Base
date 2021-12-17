using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator;

//No Abstract.
public class NpgsqlGenerator : NpgsqlMigrationsSqlGenerator
{
    private readonly ILogger<SqliteGenerator> _logger;
    //It must be public.
    public NpgsqlGenerator(
        ILoggerFactory loggerFactory,
        MigrationsSqlGeneratorDependencies dependencies,
        INpgsqlOptions migrationsAnnotations)
        : base(dependencies, migrationsAnnotations)
    {
        _logger = loggerFactory.CreateLogger<SqliteGenerator>();
        _logger.LogTrace("NpgsqlGenerator");
    }

    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "Just because")]
    protected override void Generate(
        MigrationOperation operation,
        IModel model,
        MigrationCommandListBuilder builder)
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
}