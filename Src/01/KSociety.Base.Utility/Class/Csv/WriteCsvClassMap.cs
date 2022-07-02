using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KSociety.Base.Utility.Class.Csv
{



    public static class WriteCsvClassMap<TEntity, TClassMap>
        where TEntity : class
        where TClassMap : ClassMap<TEntity>
    {
        public static bool Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TEntity> records)
        {
            var logger = loggerFactory.CreateLogger("ExportCsv");

            try
            {
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

        //public static async ValueTask<bool> ExportAsync(ILoggerFactory loggerFactory, string fileName,
        //    IEnumerable<TEntity> records)
        //{
        //    var logger = loggerFactory.CreateLogger("ExportAsyncCsv");

        //    try
        //    {
        //        await using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
        //        var writer = new CsvWriter(streamWriter, Configuration.CsvConfigurationWrite);
        //        writer.Context.RegisterClassMap<TClassMap>();
        //        await writer.WriteRecordsAsync(records).ConfigureAwait(false);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger?.LogError(ex, "WriteCsv: ");
        //    }

        //    return false;
        //}
    }
}