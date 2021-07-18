using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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
                using var streamReader = new StreamReader(fileName);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>().ToArray();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }
            return null;
        }

        public static IEnumerable<TClass> Import(ILoggerFactory loggerFactory, byte[] byteArray)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");

            try
            {
                using var streamReader = new StreamReader(new MemoryStream(byteArray));
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

                return reader.GetRecords<TClass>().ToArray();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }
            return null;
        }

        public static async IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using var streamReader = new StreamReader(fileName);
            var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

            var result = reader.GetRecordsAsync<TClass>();

            await foreach (var item in result.ConfigureAwait(false))
            {
                yield return item;
            }
        }

        public static async IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, string fileName, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using var streamReader = new StreamReader(fileName);
            var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

            var result = reader.GetRecordsAsync<TClass>(cancellationToken);

            await foreach (var item in result.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }
        }

        public static async IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, byte[] byteArray)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using var streamReader = new StreamReader(new MemoryStream(byteArray));
            var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

            var result = reader.GetRecordsAsync<TClass>();

            await foreach (var item in result.ConfigureAwait(false))
            {
                yield return item;
            }

        }

        public static async IAsyncEnumerable<TClass> ImportAsync(ILoggerFactory loggerFactory, byte[] byteArray, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");

            using var streamReader = new StreamReader(new MemoryStream(byteArray));
            var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);

            var result = reader.GetRecordsAsync<TClass>(cancellationToken);

            await foreach (var item in result.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }

        }
    }
}
