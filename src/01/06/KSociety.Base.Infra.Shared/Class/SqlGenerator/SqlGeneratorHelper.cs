// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Class.SqlGenerator
{
    using KSociety.Base.InfraSub.Shared.Class;
    using Microsoft.EntityFrameworkCore.Migrations;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Linq;

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

                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException());
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