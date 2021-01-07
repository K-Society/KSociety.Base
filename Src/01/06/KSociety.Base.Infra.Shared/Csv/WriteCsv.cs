using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Csv
{
    public class WriteCsv<TClass>
        where TClass : class
    {
        private static ILogger _logger;

        public static TClass[] Write(ILoggerFactory loggerFactory, string fileName)
        {
            _logger = loggerFactory.CreateLogger("WriteCsv");
            //TClass[] output = null;
            var csvFileName = @"." + fileName + @".csv";
            _logger.LogTrace("WriteCsv csvFileName: " + csvFileName);
            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(csvFileName));
            _logger.LogTrace("WriteCsv resourceName: " + resourceName);

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                //reader.Configuration.Delimiter = ";";
                //reader.Configuration.PrepareHeaderForMatch = (s, i) => s.ToLower();
                //reader.Configuration.GetConstructor = type => type.GetConstructors().First();

                //var result = reader.GetRecords<TClass>();
                //output = result.ToArray();

                return reader.GetRecords<TClass>().ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteCsv: ");
            }
            return null;
        }

        public static void Export(ILoggerFactory loggerFactory, string fileName, IEnumerable<TClass> records)
        {
            _logger = loggerFactory.CreateLogger("ExportCsv");

            using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
            var writer = new CsvWriter(streamWriter, Configuration.CsvConfiguration);

            //writer.Configuration.Delimiter = ";";
            //writer.WriteHeader<TClass>();
            writer.WriteRecords(records);
        }

        public static async ValueTask ExportAsync(ILoggerFactory loggerFactory, string fileName, IEnumerable<TClass> records)
        {
            _logger = loggerFactory.CreateLogger("ExportAsyncCsv");

            await using var streamWriter = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
            var writer = new CsvWriter(streamWriter, Configuration.CsvConfiguration);

            //writer.Configuration.Delimiter = ";";
            //writer.WriteHeader<TClass>();
            await writer.WriteRecordsAsync(records).ConfigureAwait(false);
        }
    }
}
