﻿using KSociety.Base.InfraSub.Shared.Class;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    public static class SqlGeneratorHelper
    {
        public static void Generate(ILogger logger,
            CreateViewOperation operation,
            MigrationCommandListBuilder builder,
            Microsoft.EntityFrameworkCore.Storage.ISqlGenerationHelper sqlHelper)
        {
            try
            {
                var assembly = AssemblyTool.GetAssemblyByName(operation.AssemblyName);

                string resourceName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(operation.ResourceSqlFileName));

                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new(stream ?? throw new InvalidOperationException());
                string result = reader.ReadToEnd();

                logger.LogDebug(result);

                builder.AppendLines(result)
                    .AppendLine(sqlHelper.StatementTerminator)
                    .EndCommand();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Generate: ");
            }
        }
    }
}
