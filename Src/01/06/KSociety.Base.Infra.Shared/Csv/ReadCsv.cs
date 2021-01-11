using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class ReadCsv<TClass>
        where TClass : class
    {

        public static TClass[] Read(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ReadCsv");
            var csvFileName = @"." + fileName + @".csv";
            logger?.LogTrace("ReadCsv csvFileName: " + csvFileName);

            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(csvFileName));
            logger?.LogTrace("ReadCsv resourceName: " + resourceName);

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>().ToArray();
            }
            catch (Exception ex)
            {
                logger?.LogError( "Error ReadCsv: " + ex.Message + " - " + ex.StackTrace);
            }
            return null;
        }

        public static IEnumerable<TClass> Import(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");
            
            try
            {
                using var streamReader = new StreamReader(fileName, Encoding.UTF8);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>().ToList();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }
            return null;
        }

        public static IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");
            IAsyncEnumerable<TClass> output = null;

            try
            {
                using var streamReader = new StreamReader(fileName, Encoding.UTF8);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                output = reader.GetRecordsAsync<TClass>();
            }
            catch (Exception ex)
            {
                logger?.LogError("ReadCsv.ImportAsync: " + ex.Message + " - " + ex.StackTrace);
            }
            return output;
        }
    }
}
