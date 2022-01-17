using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator;

public class CosmosGenerator : //CosmosMigrationsSqlGenerator
{
    private readonly ILogger<CosmosGenerator> _logger;

    //It must be public.
    public CosmosGenerator(
        ILoggerFactory loggerFactory,
        MigrationsSqlGeneratorDependencies dependencies,
        IRelationalAnnotationProvider migrationsAnnotations)
        : base(dependencies, migrationsAnnotations)
    {
        _logger = loggerFactory.CreateLogger<CosmosGenerator>();
        _logger.LogTrace("CosmosGenerator");
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