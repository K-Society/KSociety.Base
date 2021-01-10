using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class WriteCsvClassMap<TEntity, TClassMap>
        where TEntity :class
        where TClassMap : ClassMap<TEntity>
    {
        private static ILogger _logger;
        public static bool Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            _logger = loggerFactory.CreateLogger("ExportCsv");

            try
            {
                using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
                var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);
                writer.Configuration.RegisterClassMap<TClassMap>();
                writer.WriteRecords(records);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "WriteCsv: ");
            }

            return false;
        }

        public static async ValueTask<bool> ExportAsync(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            _logger = loggerFactory.CreateLogger("ExportAsyncCsv");

            try
            {
                await using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
                var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);
                writer.Configuration.RegisterClassMap<TClassMap>();
                await writer.WriteRecordsAsync(records).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "WriteCsv: ");
            }

            return false;
        }
    }
}
