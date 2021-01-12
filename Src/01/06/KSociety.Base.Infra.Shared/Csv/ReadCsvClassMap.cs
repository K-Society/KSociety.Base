using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSociety.Base.Infra.Shared.Csv
{
    public static class ReadCsvClassMap<TEntity, TClassMap>
        where TEntity : class
        where TClassMap : ClassMap<TEntity>
    {

        public static TEntity[] Read(ILoggerFactory loggerFactory, string fileName)
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
                reader.Configuration.RegisterClassMap<TClassMap>();
                return reader.GetRecords<TEntity>().ToArray();
            }
            catch (Exception ex)
            {
                logger?.LogError("Error ReadCsv: " + ex.Message + " - " + ex.StackTrace);
            }
            return null;
        }

        public static IEnumerable<TEntity> Import(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");

            try
            {
                using var streamReader = new StreamReader(fileName);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                reader.Configuration.RegisterClassMap<TClassMap>();

                return reader.GetRecords<TEntity>().ToArray();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }
            return null;
        }

        public static IEnumerable<TEntity> Import(ILoggerFactory loggerFactory, byte[] byteArray)
        {
            var logger = loggerFactory?.CreateLogger("ImportCsv");

            try
            {
                using var streamReader = new StreamReader(new MemoryStream(byteArray));
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                reader.Configuration.RegisterClassMap<TClassMap>();
                return reader.GetRecords<TEntity>().ToArray();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv: ");
            }
            return null;
        }

        public static IAsyncEnumerable<TEntity> ImportAsync(ILoggerFactory loggerFactory, string fileName)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");
            IAsyncEnumerable<TEntity> output = null;

            try
            {
                byte[] array = new byte[10];
                var test = new MemoryStream(array);
                new StreamReader(test);

                using var streamReader = new StreamReader(fileName);
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                reader.Configuration.RegisterClassMap<TClassMap>();
                output = reader.GetRecordsAsync<TEntity>();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv.ImportAsync: " + ex.Message);
            }
            return output;
        }

        public static IAsyncEnumerable<TEntity> ImportAsync(ILoggerFactory loggerFactory, byte[] byteArray)
        {
            var logger = loggerFactory?.CreateLogger("ImportAsyncCsv");
            IAsyncEnumerable<TEntity> output = null;

            try
            {
                using var streamReader = new StreamReader(new MemoryStream(byteArray));
                var reader = new CsvReader(streamReader, Configuration.CsvConfiguration);
                reader.Configuration.RegisterClassMap<TClassMap>();
                output = reader.GetRecordsAsync<TEntity>();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "ReadCsv.ImportAsync: " + ex.Message);
            }
            return output;
        }
    }
}
