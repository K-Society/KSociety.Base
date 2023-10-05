// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.Csv
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public static class WriteCsvClassMap<TEntity, TClassMap>
        where TEntity : class
        where TClassMap : ClassMap<TEntity>
    {

        #if NETSTANDARD2_0
        public static bool Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        #elif NETSTANDARD2_1
        public static bool Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        #endif
        {
            var logger = loggerFactory.CreateLogger("ExportCsv");

            try
            {
                if (records == null)
                {
                    return false;
                }

                using (var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                {
                    var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);
                    writer.Context.RegisterClassMap<TClassMap>();
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

        #if NETSTANDARD2_0
        public static async ValueTask<bool> ExportAsync(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        #elif NETSTANDARD2_1
        public static async ValueTask<bool> ExportAsync(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        #endif
        {
            var logger = loggerFactory.CreateLogger("ExportAsyncCsv");

            try
            {
                if (records == null)
                {
                    return false;
                }

                using (var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                {
                    var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);
                    writer.Context.RegisterClassMap<TClassMap>();
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
