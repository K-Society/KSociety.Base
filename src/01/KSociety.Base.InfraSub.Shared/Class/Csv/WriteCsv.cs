// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.Csv
{
    using CsvHelper;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class WriteCsv<TEntity>
        where TEntity : class
    {
        #if NETSTANDARD2_0
        public static TEntity[] Write(ILoggerFactory loggerFactory, string fileName)
        #elif NETSTANDARD2_1
        public static TEntity[] Write(ILoggerFactory loggerFactory, string fileName)
        #endif
        {
            var logger = loggerFactory.CreateLogger("WriteCsv");

            var csvFileName = @"." + fileName + @".csv";
            logger.LogTrace("WriteCsv csvFileName: {0}", csvFileName);
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(csvFileName));
            logger.LogTrace("WriteCsv resourceName: {0}", resourceName);

            try
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (var streamReader = new StreamReader(stream ?? throw new InvalidOperationException()))
                    {
                        var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                        return reader.GetRecords<TEntity>().ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WriteCsv: ");
            }

            return null;
        }

        public static bool Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            var logger = loggerFactory.CreateLogger("ExportCsv");

            try
            {
                using (var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                {
                    var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);

                    writer.WriteRecords(records);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "WriteCsv: ");
            }

            return false;
        }

        public static async ValueTask<bool> ExportAsync(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            var logger = loggerFactory.CreateLogger("ExportAsyncCsv");

            try
            {
                #if NETSTANDARD2_0
                using (var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                #elif NETSTANDARD2_1
                await using (var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                #endif
                {
                    var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);

                    await writer.WriteRecordsAsync(records).ConfigureAwait(false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "WriteCsv: ");
            }

            return false;
        }
    }
}
