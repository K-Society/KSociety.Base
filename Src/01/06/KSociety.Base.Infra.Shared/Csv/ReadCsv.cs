using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class ReadCsv<TClass>
        where TClass : class
    {
        private static ILogger _logger;
        
        public static TClass[] Read(ILoggerFactory loggerFactory, string fileName)
        {
            _logger = loggerFactory?.CreateLogger("ReadCsv");
            var csvFileName = @"." + fileName + @".csv";
            _logger?.LogTrace("ReadCsv csvFileName: " + csvFileName);

            var assembly = Assembly.GetCallingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(csvFileName));
            _logger?.LogTrace("ReadCsv resourceName: " + resourceName);

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>().ToArray();
            }
            catch (Exception ex)
            {
                _logger?.LogError( "Error ReadCsv: " + ex.Message + " - " + ex.StackTrace);
            }
            return null;
        }

        public static IEnumerable<TClass> Import(ILoggerFactory loggerFactory, string fileName)
        {
            _logger = loggerFactory?.CreateLogger("ImportCsv");
            
            try
            {
                using var streamReader = new StreamReader(fileName, Encoding.UTF8);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>().ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ReadCsv: ");
            }
            return null;
        }

        public static IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, string fileName)
        {
            _logger = loggerFactory?.CreateLogger("ImportCsv");
            IAsyncEnumerable<TClass> output = null;

            try
            {
                using var streamReader = new StreamReader(fileName, Encoding.UTF8);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                output = reader.GetRecordsAsync<TClass>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ReadCsv: ");
            }
            return output;
        }
    }
}
